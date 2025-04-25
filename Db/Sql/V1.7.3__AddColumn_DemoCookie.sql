ALTER TABLE resumedb.users
    ADD COLUMN demo_session_cookie VARCHAR(255),
    ADD COLUMN last_accessed       TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
