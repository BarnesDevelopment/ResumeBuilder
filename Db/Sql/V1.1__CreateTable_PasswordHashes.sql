create table ResumeDb.PasswordHashes
(
    id           uuid                          not null
        primary key,
    userid       uuid                          not null,
    hash         varchar(500)                         not null,
    active       bool default true                 null,
    created_date timestamp   default now() not null,
    constraint PasswordHashes_pk2
        unique (id),
    constraint PasswordHashes_Users_id_fk
        foreign key (userid) references ResumeDb.Users (id)
);

