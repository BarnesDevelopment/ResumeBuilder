CREATE TABLE ResumeDb.ResumeTree (
    id                  varchar(36)             not null primary key,
    active              tinyint(1)   default 1  not null,
    userid              varchar(36)             not null,
    parentid            varchar(36)                 null,
    placementorder      int                     not null,
    content             varchar(5000)           not null,
    section_type        varchar(255)            not null,
    
    constraint ResumeTree_pk
        unique (id),
    constraint userid
        foreign key (userid) references ResumeDb.Users (id),
    constraint parentid
        foreign key (parentid) references ResumeDb.ResumeTree (id)
)