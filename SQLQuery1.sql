create database effort_tracking_system;

use effort_tracking_system;

create table Admin(
	admin_id int identity(1,1) primary key,
	name varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null,
	Role varchar(10) not null
);
drop table Admin
-- Inserting a sample admin into the Admin table
INSERT INTO Admin (name, email, password, Role)
VALUES ('Admin', 'admin@gmail.com', 'admin123', 'admin');

create table Users(
	user_id int identity(1,1) primary key,
	first_name varchar(50) not null,
	last_name varchar(50) not null,
	designation varchar(50) not null,
	email varchar(50) not null,
	password varchar(100) not null
);
-- Inserting sample data into the Users table
INSERT INTO Users (first_name, last_name, designation, email, password)
VALUES
    ('Alice', 'Johnson', 'Software Engineer', 'alice@gmail.com', 'Alice123'),
    ('Bob', 'Smith', 'Data Analyst', 'bob@gmail.com', 'Bob123'),
    ('Charlie', 'Williams', 'UI/UX Designer', 'charlie@gmail.com', 'Charlie123'),
    ('David', 'Brown', 'Project Manager', 'david@gmail.com', 'David123');

drop table Users

create table Project(
	project_id int identity(1,1) primary key,
	name varchar(100) not null
);
-- Inserting sample data into the Project table
INSERT INTO Project (name)
VALUES
    ('Project A'),
    ('Project B'),
    ('Project C'),
    ('Project D');

drop table Project
create table Task(
	task_id int identity(1,1) primary key,
	task_name varchar(50) not null
);
INSERT INTO Task (task_name)
VALUES
    ('Task A'),
    ('Task B'),
    ('Task C'),
    ('Task D');
drop table Task
create table Shifts(
	shift_id int identity(1,1) primary key,
	shift_name varchar(50) not null,
	start_time time not null,
	end_time time not null
);
-- Inserting sample data into the Shifts table
INSERT INTO Shifts (shift_name, start_time, end_time)
VALUES
    ('Morning Shift', '08:00:00', '16:00:00'),
    ('Afternoon Shift', '12:00:00', '20:00:00'),
    ('Night Shift', '20:00:00', '04:00:00');

drop table Shifts
create table Effort(
	effort_id int identity(1,1) primary key,
	assign_task_id int references Assign_Task(assign_task_id),
	shift_id INT REFERENCES Shifts(shift_id),
	hours_worked int not null,
	date_time date default getdate(),
	status varchar(20) default 'pending'
);

INSERT INTO Effort (assign_task_id,shift_id, hours_worked, date_time, status)
VALUES
    (1,1, 9, '2023-08-18', 'Approved'),
    (1,1, 9, '2023-08-21', 'Approved'),
    (3,3, 9, '2023-08-23', 'Approved'),
    (1,1, 9, '2023-08-22', 'Approved'),
    (2,2, 9, '2023-08-21', 'Approved'),
    (2,2, 9, '2023-08-22', 'Approved'),
    (2,2, 9, '2023-08-23', 'Approved'),
    (1,1, 9, '2023-08-23', 'Approved');
drop table Effort

create table Leave(
	leave_id int identity(1,1) primary key,
	user_id int references Users(user_id),
	date date not null,
	reason varchar(max) not null,
	status varchar(20) not null default 'Not Approved'
);
drop table Leave
CREATE TABLE Shift_Change (
    shift_Change_id int identity(1,1) primary key,
    user_id int references Users(user_id),
    assigned_shift_id int references Shifts(shift_id),
    date date not null,
    new_shift_id int references Shifts(shift_id),
    reason varchar(max) not null,
   status varchar(20)
);
drop table Shift_Change
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
INSERT INTO Assign_Task (user_id, project_id, task_id, shift_id, allocated_hours, assignmentdate, Status)
VALUES
    (1, 1, 1, 1, 45, '2023-08-18', 'Pending'),
    (2, 2, 2, 2, 45, '2023-08-21', 'Pending'),
    (3, 1, 3, 3, 45, '2023-08-23', 'Pending');
    
drop table Assign_Task

select * from admin
select * from users
select * from project
select * from task
select * from shifts
select * from Leave
select * from Shift_Change
select * from effort
select * from Assign_Task