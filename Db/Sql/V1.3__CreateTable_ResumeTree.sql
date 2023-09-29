CREATE TABLE ResumeDb.ResumeTree (
     id                  uuid             not null primary key,
     active              bool   default true  not null,
     userid              uuid             not null,
     parentid            uuid                 null,
     placementorder      int                     not null,
     content             varchar(5000)           not null,
     sectiontype         varchar(255)            not null,
     depth               int                     not null default 0,

     constraint ResumeTree_pk
         unique (id),
     constraint ResumeTree_userid
         foreign key (userid) references ResumeDb.Users (id),
     constraint ResumeTree_parentid
         foreign key (parentid) references ResumeDb.ResumeTree (id)
);