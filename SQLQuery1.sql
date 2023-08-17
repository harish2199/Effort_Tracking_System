create database effort_tracking_system;

use effort_tracking_system;

create table admin(
	admin_id int identity(1,1) primary key,
	name varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null,
	Role varchar(10) not null
);
drop table admin
create table users(
	user_id int identity(1,1) primary key,
	first_name varchar(50) not null,
	last_name varchar(50) not null,
	designation varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null
);

create table project(
	project_id int identity(1,1) primary key,
	name varchar(100) not null
);

create table task(
	task_id int identity(1,1) primary key,
	task_name varchar(50) not null,
	project_id int references project(project_id)
);

create table shifts(
	shift_id int identity(1,1) primary key,
	shift_name varchar(50) not null,
	start_time time not null,
	end_time time not null
);

create table effort(
	effort_id int identity(1,1) primary key,
	user_id int references users(user_id),
	project_id int references project(project_id),
	task_id int references task(task_id),
	shift_id int references shifts(shift_id),
	hours_worked int not null,
	date_time datetime not null,
	status varchar(20) not null
);

create table unavailability(
	unavailability_id int identity(1,1) primary key,
	user_id int references users(user_id),
	date datetime not null,
	shift_id int references shifts(shift_id),
	reason varchar(max) not null,
	status varchar(20) not null
);

create table user_task_assignment(
	assignment_id int identity(1,1) primary key,
	user_id int references users(user_id),
	project_id int references project(project_id),
	task_id int references task(task_id),
	shift_id int references shifts(shift_id),
	hours int not null
);

-- Sample data for Admin table
INSERT INTO admin (name, email, password,Role)
VALUES ('Admin Name', 'admin@example.com', 'admin123','Admin');

-- Sample data for User table
INSERT INTO users (first_name, last_name, designation, email, password)
VALUES ('John', 'Doe', 'Developer', 'john@example.com', 'john123'),
       ('Jane', 'Smith', 'Designer', 'jane@example.com', 'jane123');

-- Sample data for Project table
INSERT INTO project (name)
VALUES ('Project A'),
       ('Project B');

-- Sample data for Task table
INSERT INTO task (task_name, project_id)
VALUES ('Task 1', 1),
       ('Task 2', 1),
       ('Task 1', 2),
       ('Task 2', 2);

-- Sample data for Shifts table
INSERT INTO shifts (shift_name, start_time, end_time)
VALUES ('Morning', '08:00:00', '16:00:00'),
       ('Evening', '16:00:00', '00:00:00'),
       ('Night', '00:00:00', '08:00:00');

-- Sample data for Effort table
INSERT INTO effort (user_id, project_id, task_id, shift_id, hours_worked, date_time, status)
VALUES (1, 1, 1, 1, 8, '2023-08-15 08:00:00', 'Completed'),
       (2, 1, 2, 1, 6, '2023-08-15 08:00:00', 'In Progress');

-- Sample data for UnAvailability table
INSERT INTO unavailability (user_id, date, shift_id, reason, status)
VALUES (1, '2023-08-16', 1, 'Sick Leave', 'Pending'),
       (2, '2023-08-17', 2, 'Vacation', 'Approved');

-- Sample data for UserTaskAssignment table
INSERT INTO user_task_assignment (user_id, project_id, task_id, shift_id, hours)
VALUES (1, 1, 1, 1, 4),
       (1, 1, 2, 1, 3),
       (2, 1, 1, 2, 5);


select * from admin
select * from users
select * from project
select * from task
select * from shifts
select * from effort
select * from unavailability
select * from user_task_assignment

drop table user_task_assignment

update unavailability set status = 'not approved' where unavailability_id = 1