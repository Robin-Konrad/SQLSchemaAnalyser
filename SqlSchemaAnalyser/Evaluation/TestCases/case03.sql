CREATE TABLE departments (
    id INT PRIMARY KEY,
    dept_name VARCHAR(100) NOT NULL,
    manager_name VARCHAR(100)
);

CREATE TABLE offices (
    id INT PRIMARY KEY,
    office_name VARCHAR(100),
    city VARCHAR(100),
    country VARCHAR(100)
);

CREATE TABLE employees (
    id INT PRIMARY KEY,
    dept_id INT NOT NULL,
    office_id INT,
    emp_name VARCHAR(100),
    emp_email VARCHAR(150),
    department_name VARCHAR(100),
    salary DECIMAL(10,2),
    hire_date DATETIME,
    status VARCHAR(20)
);

ALTER TABLE employees ADD CONSTRAINT fk_emp_dept FOREIGN KEY (dept_id) REFERENCES departments(id);
ALTER TABLE employees ADD CONSTRAINT fk_emp_office FOREIGN KEY (office_id) REFERENCES offices(id);

CREATE TABLE projects (
    id INT PRIMARY KEY,
    project_name VARCHAR(150) NOT NULL,
    dept_id INT,
    start_date DATETIME,
    end_date DATETIME,
    proj_status VARCHAR(20)
);

ALTER TABLE projects ADD CONSTRAINT fk_proj_dept FOREIGN KEY (dept_id) REFERENCES departments(id);

CREATE TABLE employee_projects (
    id INT PRIMARY KEY,
    emp_id INT NOT NULL,
    proj_id INT NOT NULL,
    role_on_project VARCHAR(100),
    assigned_date DATETIME
);

ALTER TABLE employee_projects ADD CONSTRAINT fk_ep_employee FOREIGN KEY (emp_id) REFERENCES employees(id);
ALTER TABLE employee_projects ADD CONSTRAINT fk_ep_project FOREIGN KEY (proj_id) REFERENCES projects(id);

CREATE INDEX idx_employees_hire_date ON employees(hire_date);
CREATE UNIQUE INDEX idx_employees_email ON employees(emp_email);