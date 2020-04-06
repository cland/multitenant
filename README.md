# multitenant
 Rest API that uses JWT token bearer authentication and works fine. I am now trying to adjust to handle multi-tenancy where by each tenant has their own database in Postgresql (can be MySql)

This is just a test platform 


# THE PROBLEM:

Tried to seed roles and users within ApplicationDbContext class, OnModelCreating. I now get the error TenantId missing for IdentityUser object. I solved this error for roles by adding "TenantId" but I can't do the same for IdentityUser object as it says "TenantId" is not valid.

Also notiticed that my SeedDatabase class problem is with the instantiation of the UserManager object; it uses the default ApplicationDbContext in Startup.cs that does NOT have any TenantInfo object and therefore the CONNECTIONSTRING is null.

# FIRST UPDATE
Got something working...not tested authentication yet. Cannot run the get in postman for some reason BUT json result returned fine in the browser....struggle continues.

