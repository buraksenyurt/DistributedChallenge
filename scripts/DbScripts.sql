CREATE TABLE report (
    report_id SERIAL PRIMARY KEY,
    trace_id UUID NOT NULL,
    title VARCHAR(50) NOT NULL,
    expression VARCHAR(200) NOT NULL,
    employee_id VARCHAR(20) NOT NULL,
    document_id VARCHAR(60) NOT NULL,
    insert_time TIMESTAMP,
    expire_time TIMESTAMP,
    archived BOOLEAN DEFAULT False,
    deleted BOOLEAN DEFAULT False
);

CREATE TABLE report_document (
    report_document_id SERIAL PRIMARY KEY,
    fk_report_id int NOT NULL,
    Content BYTEA NOT NULL
);

CREATE TABLE employee (
    employee_id SERIAL PRIMARY KEY,
	full_name VARCHAR(100) NOT NULL,
	email VARCHAR(35) NOT NULL,
    title VARCHAR(50) NOT NULL,
	registration_id VARCHAR(10) NOT NULL,
    password_hash VARCHAR(60) NOT NULL
);

CREATE TABLE employee_tokens (
    registration_id VARCHAR(10) NOT NULL,
    token VARCHAR(2048) NOT NULL,
    insert_time TIMESTAMP,
    expire_time TIMESTAMP,
    CONSTRAINT employee_tokens_registration_id_unique UNIQUE (registration_id)
);