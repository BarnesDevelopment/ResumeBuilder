DROP TABLE resumedb.cookies;
DROP TABLE resumedb.passwordhashes;

ALTER TABLE resumedb.users
    DROP CONSTRAINT Users_pk4,
    DROP CONSTRAINT Users_pk5,
    DROP COLUMN username,
    DROP COLUMN email,
    DROP COLUMN firstname,
    DROP COLUMN lastname,
    DROP COLUMN updated_date;
