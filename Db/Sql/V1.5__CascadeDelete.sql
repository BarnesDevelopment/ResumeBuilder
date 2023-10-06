ALTER TABLE ResumeDb.ResumeTree
    DROP CONSTRAINT ResumeTree_parentid,
    ADD CONSTRAINT FK_ResumeTree_parentid
        FOREIGN KEY (parentid)
        REFERENCES ResumeDb.ResumeTree (id)
        ON DELETE CASCADE;