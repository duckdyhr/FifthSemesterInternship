insert into Year (value) values (2020)
delete from Student where id = 3
insert into Company (name, phone, website) values ('Splash', '34534534', 'Splash.dk')
insert into Supervisor (name, email, phone) values ('Louise Dyhr', 'louis@gmail.com', '20307786')
insert into Contact (name, phone, email, CompanyID) values('Vilde Leiros Dyhr', '56741234', 'vilde@AQweb.com', 1)
delete from student where 1=1
insert into Student (name, year, season, SupervisorID, CompanyID, mainProjectTitle) values ('Maria Ladegård', 2016, 'Fall', 1, 1, 'På barsel')
update Student set season='Autumn' where season='Fall'