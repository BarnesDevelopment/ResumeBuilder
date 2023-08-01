create table ResumeDb.PasswordHashes
(
    id           varchar(36)                          not null
        primary key,
    userid       varchar(36)                          not null,
    hash         varchar(500)                         not null,
    active       tinyint(1) default 1                 null,
    created_date datetime   default CURRENT_TIMESTAMP not null,
    constraint PasswordHashes_pk2
        unique (id),
    constraint PasswordHashes_Users_id_fk
        foreign key (userid) references ResumeDb.Users (id)
);

