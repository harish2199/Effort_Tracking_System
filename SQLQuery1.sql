create database effort_tracking_system;

use effort_tracking_system;

create table Admin(
	admin_id int identity(1,1) primary key,
	name varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null,
	Role varchar(10) not null
);
create table Users(
	user_id int identity(1,1) primary key,
	first_name varchar(50) not null,
	last_name varchar(50) not null,
	designation varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null
);
drop table users
create table Project(
	project_id int identity(1,1) primary key,
	name varchar(100) not null
);
create table Task(
	task_id int identity(1,1) primary key,
	task_name varchar(50) not null
);
create table Shifts(
	shift_id int identity(1,1) primary key,
	shift_name varchar(50) not null,
	start_time time not null,
	end_time time not null
);
create table Effort(
	effort_id int identity(1,1) primary key,
	assign_task_id int references Assign_Task(assign_task_id),
	hours_worked int not null,
	date_time date default getdate(),
	status varchar(20) default 'pending'
);
INSERT INTO Effort (assign_task_id, hours_worked, date_time, status)
VALUES
    (1, 9, '2023-08-15', 'Approved'),
    (1, 9, '2023-08-16', 'Approved'),
    (1, 9, '2023-08-17', 'Approved'),
    (1, 9, '2023-08-18', 'Approved'),
    (2, 9, '2023-08-14', 'Approved'),
    (2, 9, '2023-08-15', 'Approved'),
    (2, 9, '2023-08-16', 'Approved'),
    (2, 9, '2023-08-17', 'Approved'),
    (2, 9, '2023-08-18', 'Approved'),
    (2, 9, '2023-08-21', 'Approved');
    
drop table effort

INSERT INTO effort(user_id, project_id, task_id, shift_id, hours_worked)
VALUES (1, 1, 1, 1,9);

create table Leave(
	leave_id int identity(1,1) primary key,
	user_id int references Users(user_id),
	date date not null,
	shift_id int references Shifts(shift_id),
	reason varchar(max) not null,
	status varchar(20) not null default 'Not Approved'
);
drop table Leave
create table Assign_Task (
    assign_task_id int identity(1,1) primary key,
    user_id int references users(user_id),
    project_id int references project(project_id),
    task_id int references task(task_id),
    shift_id int references shifts(shift_id),
    allocated_hours int not null,
	assignmentdate date default getdate(),
	Status varchar(20) default 'Pending'
);

drop table Assign_Task

-- Insert sample data into Admin table
INSERT INTO Admin (name, email, password, Role)
VALUES ('Admin 1', 'admin1@example.com', 'adminpass1', 'Admin'),
       ('Admin 2', 'admin2@example.com', 'adminpass2', 'Admin');

-- Insert sample data into Users table
INSERT INTO Users (first_name, last_name, designation, email, password)
VALUES ('John', 'Doe', 'Developer', 'john@example.com', 'userpass1'),
       ('Jane', 'Smith', 'Designer', 'jane@example.com', 'userpass2');

-- Insert sample data into Project table
INSERT INTO Project (name)
VALUES ('Project A'),
       ('Project B');

-- Insert sample data into Task table
INSERT INTO Task (task_name)
VALUES ('Task 1'),
       ('Task 2');

-- Insert sample data into Shifts table
INSERT INTO Shifts (shift_name, start_time, end_time)
VALUES ('Morning Shift', '08:00:00', '16:00:00'),
       ('Night Shift', '20:00:00', '04:00:00');

-- Insert sample data into Leave table
INSERT INTO Leave (user_id, date, shift_id, reason, status)
VALUES (1, '2023-08-20', 1, 'Vacation', 'Not Approved'),
       (2, '2023-08-22', 2, 'Sick Leave', 'Pending');

-- Insert sample data into Assign_Task table
INSERT INTO Assign_Task (user_id, project_id, task_id, shift_id, allocated_hours, assignmentdate, Status)
VALUES (1, 1, 1, 1, 45, GETDATE(), 'Pending'),
       (2, 2, 2, 2, 45, GETDATE(), 'Approved');

select * from admin
select * from users
select * from project
select * from task
select * from shifts
select * from Leave
select * from effort
select * from Assign_Task

delete Assign_Task where assign_task_id =4
delete effort where effort_id =15