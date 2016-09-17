--*****************************
--StudentSupervisorAssignment
--Student-Supervisor-Company information
--*****************************
select substring(st.name, 1, 30) as Name, 
	st.class as Class, 
	substring(su.name,1, 30) as Supervisor, 
	substring(su.email, 1, 15) as Email, 
	substring(c.name, 1, 25) as Company, 
	st.comments
from student st
inner join supervisor su
  on su.id = st.SupervisorID
left join company c
  on c.id = st.CompanyID
where st.year = 2016 and st.season = 'Autumn' 
order by su.name, c.name;
-- order by st.class, st.name, c.name;

--*****************************
--StudentCompanyAssignment
--*****************************

select substring(st.name, 1, 30) as Studentname, 
	st.class as Class, 
	c.name as Company, 
	substring(st.comments, 1, 20) as Comments
from student st
left join Company c
  on st.CompanyID = c.id
  where st.year = 2016 and st.season = 'Autumn'
    order by st.class;
--   order by c.name;

--How many students per season already have an agreement with a company
select st.year, st.season, count(*)
from student st, Company c
  where st.CompanyID = c.id
  and c.Name <> 'Erhvervsakademi Aarhus'
  group by st.year, st.season
  order by st.year, st.season DESC

 --How many students per season have Erhvervsakademi Århus registrered as their comapany
select st.year, st.season, count(*)
from student st, Company c
  where st.CompanyID = c.id
  and c.Name = 'Erhvervsakademi Aarhus'
  group by st.year, st.season
    order by st.year, st.season DESC
	
--*****************************
--MainProjectOverviewSigne
--*****************************

select substring(st.name, 1, 40) as name, 
	substring(st.class, 1, 7) as Class, 
	substring(str(m.groupNo, 2, 0), 1, 2) as Grp, 
	substring(su.name,1, 30) as Supervisor, 
	substring(m.title, 1,55) as ProjectTitle, 
	substring(c.name, 1, 25) as InternshipCompany
from student st, Company c, MainProject m, Supervisor su
where su.id = m.SupervisorID
-- and c.id = m.CompanyID
and c.id = m.CompanyID
and st.mainProjectTitle = m.title
and m.year = 2016 and m.season = 'Spring'  
order by groupNo, su.name
--,c.name;

--inner join supervisor su
--  on su.id = m.SupervisorID
--left join company c
--  on c.id = st.CompanyID
--left join MainProject m
--  on st.mainProjectTitle = m.title 

--*****************************
--Companies med kontaktpersoner
--*******************************
--select c.name, address, zipcode, co.name from Company c, Contact co
--where c.id = co.CompanyID;
--select substring(c.name,1,30) as Companyname, substring(address,1,30) as Address, zipcode, substring(Po.Bynavn,1,20) as City, c.website, comments as Comments from Company c, Postnr Po
--where c.zipcode = Po.Postnr#;
-- select substring(c.name,1,30) as Companyname, substring(address,1,30) as Address, zipcode, substring(Po.Bynavn,1,20) as City from Company c, Postnr Po
-- where c.zipcode = Po.Postnr#;

(select substring(c.name,1,30) as Companyname, substring(address,1,30) as Address, zipcode, substring(Po.Bynavn,1,20) as City, c.website as WebAdress
from Company c inner join Postnr Po
on c.zipcode = Po.Postnr#  left join Offering o
  on o.CompanyID = c.id)
--  order by c.zipcode)
except
(select substring(c.name,1,30) as Companyname, substring(address,1,30) as Address, zipcode, substring(Po.Bynavn,1,20) as City, c.website as WebAdress
from Company c, Postnr Po, Offering o
where Po.Postnr# = c.zipcode
and o.CompanyID = c.id
and o.year = 2345)
order by c.zipcode;


--  SELECT a.au_lname, a.au_fname, t.title
--FROM authors a INNER JOIN titleauthor ta
--   ON a.au_id = ta.au_id JOIN titles t
--   ON ta.title_id = t.title_id
-- WHERE t.type = 'trad_cook'
-- ORDER BY t.title ASC