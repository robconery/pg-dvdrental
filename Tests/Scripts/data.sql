-- default roles
insert into roles (id, description) values(10, 'Administrator');
insert into roles (id, description) values(99, 'User');

-- add an admin
select * from register('admin@test.com','password','password','127.0.0.1','approved','Big','Admin');
--set them as an Admin
select add_user_to_role('admin@test.com',10);

-- add a regular user
select * from register('test@test.com','password','password','127.0.0.1','approved','Test','User');
