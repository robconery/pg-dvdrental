
--- AUTH LOGIN
create or replace function authenticate(
  pkey varchar(255),
  ptoken varchar(255),
  prov varchar(50)
)
returns TABLE (
	user_id bigint,
	session_id varchar(255),
	message varchar(255),
	email varchar(255),
    success boolean,
    public_name varchar(255)
) as

$$
DECLARE
  return_id bigint;
  session_id varchar(36);
  message varchar(255);
  return_name varchar(255);
  success boolean;
  found_user users;
  session_length int;

BEGIN
  select false into success;

  -- find the user
  select user_logins.user_id from user_logins
  		where user_logins.provider_key=pkey
  		AND user_logins.provider_token = crypt(ptoken,provider_token)
  		AND user_logins.provider=prov into return_id;

  if not return_id is NULL then

    --yay!
    select true into success;

    select * from users where users.id=return_id into found_user;
    select 'Successfully authenticated' into message;
    select found_user.id into return_id;

    --a nice return name
	if found_user.first is null then
	  select found_user.email into return_name;
	else
	  select(found_user.first || ' ' || found_user.last) into return_name;
	end if;

	-- update user stats
	update users set
	signin_count = signin_count + 1,
	current_signin_at = now(),
	last_signin_at = current_signin_at
	where id = return_id;

	-- deal with old sessions
	if exists(select id from sessions where sessions.user_id=return_id and expires_at >= now() ) then
	  update sessions set expires_at = now() where sessions.user_id=return_id and expires_at >= now();
	end if;

	-- since this is a new login, create a new session - this will invalidate
	-- any shared login sessions where 2 people use the same account
	select session_length_weeks into session_length from membership_settings limit 1;

	--create a session
	insert into sessions(user_id, created_at, expires_at)
	values (return_id, now(), now() + interval '1 week' * session_length) returning id into session_id;

	-- add a log entry
	insert into logs(subject, entry, user_id, created_at)
	values('Authentication', 'Successfully logged in', return_id, now());


  else
    select 'Invalid username or password' into message;
  end if;

  return query
  select return_id, session_id, message, pkey, success, return_name;
END;
$$ LANGUAGE PLPGSQL;

--- REGISTRATION
create or replace function register(
  new_email varchar(255),
  pass varchar(255),
  confirm varchar(255),
  ip inet,
  chosen_status varchar(12),
  first varchar(25),
  last varchar(25)
)

returns TABLE (
	new_id bigint,
	message varchar(255),
	email varchar(255),
	success BOOLEAN,
	status varchar(12),
	email_validation_token varchar(36)) as
$$
DECLARE
	new_id bigint;
  message varchar(255);
	hashedpw varchar(255);
	success BOOLEAN;
	return_email varchar(255);
	return_status varchar(12);
	validation_token varchar(36);
  verify_email boolean;
BEGIN
	select chosen_status into return_status;
	select false into success;

	select email_validation_required from membership_settings limit(1) into verify_email;

  if return_status is null or verify_email then
		select 'pending' into return_status;
	end if;

	select new_email into return_email;

	if(pass <> confirm) THEN
		select 'Password and confirm do not match' into message;

	elseif exists(select users.email from users where users.email=return_email)  then
		select 0 into new_id;
		select 'Email exists' into message;
	ELSE
		select true into success;
		--encrypt password
		SELECT crypt(pass, gen_salt('bf', 10)) into hashedpw;
		select random_value(36) into validation_token;

		insert into users(email, created_at, ip, status, first, last, email_validation_token)
		VALUES(new_email, now(), ip, return_status, first, last,validation_token) returning id into new_id;

		select 'Successfully registered' into message;

		--add login bits to user_logins
		insert into user_logins(user_id, provider, provider_key, provider_token)
		values(new_id, 'local',return_email,hashedpw);

		--add auth token
		insert into user_logins(user_id, provider, provider_key, provider_token)
        values(new_id, 'token',null,validation_token);

		-- add them to the users role
		insert into users_roles(user_id, role_id)
		VALUES(new_id, 99);

		--add log entry
		insert into logs(subject,entry,user_id, created_at)
		values('Registration','Added to system, set role to User',new_id,now());
	end if;

return query
select new_id, message, new_email, success, return_status, validation_token;
END;
$$ LANGUAGE PLPGSQL;


create or replace function get_current_user(session_id bigint)
returns TABLE(
	user_id bigint,
	email varchar(255),
	first varchar(50),
	last varchar(50),
    last_signin_at timestamptz,
	profile json,
	status varchar(20))
as
$$
DECLARE
  found_id bigint;
  found_user users;
begin

  --session exist?
  if exists(select id from sessions where id=session_id AND expires_at >= now()) then
	--get the user record
	select sessions.user_id into found_id from sessions where id=session_id;
	select * from users where id=found_id into found_user;

	--reset the expiration on the session
	update sessions set expires_at = now() + interval '2 weeks' where sessions.id = session_id;

  end if;

  return query
  select found_user.id,
	found_user.email,
	found_user.first,
	found_user.last,
	found_user.last_signin_at,
	found_user.profile,
	found_user.status;

end;
$$ language plpgsql;


create or replace function add_login(
  uid bigint,
  pkey varchar(50),
  ptoken varchar(255),
  prov varchar(50)
)
returns table(success boolean, login_id bigint, user_id bigint) as
$$
DECLARE
  ok boolean;
  new_id bigint;
BEGIN

  --replace the provider for this user completely
  delete from user_logins where uid = user_logins.user_id AND provider = prov;
  select false into ok;
  --no hijacking
  if not exists(select id from user_logins where provider_key = pkey and provider_token=ptoken) then

    --add the login
    insert into user_logins(user_id,provider_key, provider_token, provider)
    values (uid, pkey,ptoken,prov) returning id into new_id;

    --add log entry
    insert into logs(subject,entry,user_id, created_at)
    values('Authentication','Added ' || prov || ' login',uid,now());

    select true into ok;
  end if;

  return query
  select ok, new_id, uid;
END;
$$
language plpgsql;

create or replace function add_user_to_role(user_email varchar(255), new_role_id int, out succeeded bool)
as $$
DECLARE
	found_user_id bigint;
BEGIN
	select false into succeeded;
	if exists(select id from users where email=user_email) then
		select id into found_user_id from users where email=user_email;
		if not exists(select user_id from users_roles where user_id = found_user_id and role_id=new_role_id) then
			insert into users_roles(user_id, role_id) values (found_user_id, new_role_id);
			select true into succeeded;
		end if;
	end if;
END;
$$ LANGUAGE plpgsql;

