drop table if exists membership_settings;
drop table if exists logs;
drop table if exists notes;
drop table if exists users_roles;
drop table if exists roles;
drop table if exists user_logins;
drop table if exists sessions;
drop table if exists users;

drop sequence if exists global_id_seq;
create sequence global_id_seq;

create or replace function random_value(len int, out result varchar(32))
as
$$
BEGIN
	SELECT substring(md5(random()::text),0, len) into result;
END
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION id_generator(OUT result bigint) AS $$
DECLARE
    our_epoch bigint := 1314220021721;
    seq_id bigint;
    now_millis bigint;
    shard_id int := 1;
BEGIN
	SELECT nextval('global_id_seq')%1024 INTO seq_id;

    SELECT FLOOR(EXTRACT(EPOCH FROM clock_timestamp()) * 1000) INTO now_millis;
    result := (now_millis - our_epoch) << 23;
    result := result | (shard_id << 10);
    result := result | (seq_id);
END;
$$ LANGUAGE PLPGSQL;


create table membership_settings(
  id serial primary key not null,
  session_length_weeks int not null default 2,
  email_validation_required boolean not null default false,
  email_from_name varchar(50) not null,
  email_from_address varchar(125) not null,
  github_login boolean default false,
  facebook_login boolean default false,
  twitter_login boolean default false,
  google_login boolean default false
);

insert into membership_settings(email_from_name, email_from_address)
values ('Admin', 'admin@example.com');

create table users(
  id bigint primary key not null unique DEFAULT id_generator(),
  first varchar(25),
  last varchar(25),
  user_key varchar(12) not null unique default random_value(12),
  email_validated boolean not null default false,
  email_validated_at timestamptz,
  email_validation_token varchar(36) default random_value(36),
  reset_password_token varchar(36),
  reset_password_token_set_at timestamptz,
  current_signin_at timestamptz,
  last_signin_at  timestamptz,
  email varchar(255) unique not null,
  search tsvector,
  created_at timestamptz DEFAULT current_timestamp,
  signin_count int,
  ip inet,
  status varchar(12) default 'pending',
  profile json
);

create table user_logins (
  id bigint primary key not null unique DEFAULT id_generator(),
  user_id bigint not null references users(id) on delete cascade,
  provider varchar(50) not null default 'local',
  provider_key varchar(255),
  provider_token varchar(255) not null
);

create table sessions(
  id bigint primary key not null unique DEFAULT id_generator(),
  user_id bigint not null references users(id) on delete cascade,
  created_at timestamptz not null DEFAULT current_timestamp,
  expires_at timestamptz not null 
);

create table roles(
 id integer primary key not null,
 description varchar(24) not null
);

create table users_roles(
  user_id bigint not null references users(id) on delete cascade,
  role_id int not null references roles(id) on delete cascade,
  primary key (user_id, role_id)
);

CREATE TRIGGER users_search_vector_refresh
  BEFORE INSERT OR UPDATE ON users
FOR EACH ROW EXECUTE PROCEDURE
  tsvector_update_trigger(search, 'pg_catalog.english',  email, first, last);


create table logs(
  id serial primary key not null,
  subject varchar(255) not null,
  user_id bigint not null references users(id) on delete cascade,
  entry text not null,
  created_at timestamptz
);

create table notes(
  id serial primary key not null,
  user_id bigint not null references users(id) on delete cascade,
  note text not null,
  created_at timestamptz
);




--crypto in the DB yo
--create extension pgcrypto