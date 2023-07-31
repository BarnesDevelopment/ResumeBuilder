create table ResumeDb.PasswordHashes
(
    id     varchar(36)  not null
        primary key,
    userid varchar(36)  not null,
    hash   varchar(500) not null,
    constraint PasswordHashes_pk2
        unique (id)
);

alter table PasswordHashes
    add constraint PasswordHashes_Users_id_fk
        foreign key (userid) references Users (id);