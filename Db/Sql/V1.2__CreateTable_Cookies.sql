create table ResumeDb.Cookies
(
    cookie     uuid          not null
        primary key,
    active     bool default true not null,
    expiration timestamp             not null,
    userid     uuid          not null,
    constraint Cookies_pk2
        unique (cookie),
    constraint userid
        foreign key (userid) references ResumeDb.Users (id)
);

