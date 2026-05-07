UPDATE Companies SET LeaderId = NULL;
UPDATE Divisions SET LeaderId = NULL;
UPDATE Projects SET LeaderId = NULL;
UPDATE Departments SET LeaderId = NULL;

DELETE FROM Departments;
DELETE FROM Projects;
DELETE FROM Divisions;
DELETE FROM Employees;
DELETE FROM Companies;

DBCC CHECKIDENT ('Departments', RESEED, 0);
DBCC CHECKIDENT ('Projects', RESEED, 0);
DBCC CHECKIDENT ('Divisions', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
DBCC CHECKIDENT ('Companies', RESEED, 0);