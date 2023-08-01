create table ResumeDb.Cookies
(
    cookie     varchar(36)          not null
        primary key,
    active     tinyint(1) default 1 not null,
    expiration datetime             not null,
    userid     varchar(36)          not null,
    constraint Cookies_pk2
        unique (cookie),
    constraint userid
        foreign key (userid) references ResumeDb.Users (id)
);

