CREATE TABLE Documents (
    Id SERIAL PRIMARY KEY,
    TraceId UUID NOT NULL,
    ReportTitle VARCHAR(50) NOT NULL,
    EmployeeId VARCHAR(20) NOT NULL,
    DocumentId VARCHAR(255) NOT NULL,
    InsertTime TIMESTAMP,
    ExpireTime TIMESTAMP,
    Content BYTEA NOT NULL
);