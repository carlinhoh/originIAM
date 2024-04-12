CREATE TABLE EligibilityFileReport (
    Id SERIAL PRIMARY KEY,
    EmployerId VARCHAR(20),
    RecordData JSONB NOT NULL,
    Status VARCHAR(50),
    ProcessedAt TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'utc')
);

CREATE INDEX idx_eligibilityfilereport_employerid ON EligibilityFileReport (EmployerId);
CREATE INDEX idx_eligibilityfilereport_status ON EligibilityFileReport (Status);
