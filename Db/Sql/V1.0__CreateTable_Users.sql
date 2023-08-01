create table ResumeDb.Users
(
    username     varchar(255)                       not null,
    email        varchar(255)                       not null,
    id           varchar(36)                        not null
        primary key,
    firstname    varchar(255)                       not null,
    lastname     varchar(255)                       not null,
    created_date datetime default CURRENT_TIMESTAMP not null,
    updated_date datetime default CURRENT_TIMESTAMP not null,
    constraint Users_pk2
        unique (id),
    constraint Users_pk4
        unique (username),
    constraint Users_pk5
        unique (email)
);

