using BugTracker.Models;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BugTracker.Data
{
    public static class DataUtility
    {
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;

        private static int portfolioId;
        private static int blogId;
        private static int bugTrackerId;
        private static int movieId;

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync (IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();

            await dbContextSvc.Database.MigrateAsync();

            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc);
            await SeedDemoUsersAsync(userManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefaultProjectsAsync(dbContextSvc);
            await SeedDefaultTicketsAsync(dbContextSvc, userManagerSvc);
            await SeedDefaultNotificationTypeAsync(dbContextSvc);

        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(nameof(BTRole.Admin)));
            await roleManager.CreateAsync(new IdentityRole(nameof(BTRole.ProjectManager)));
            await roleManager.CreateAsync(new IdentityRole(nameof(BTRole.Developer)));
            await roleManager.CreateAsync(new IdentityRole(nameof(BTRole.Submitter)));
            await roleManager.CreateAsync(new IdentityRole(nameof(BTRole.DemoUser)));
        }

        public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>() {
                    new Company() { Name = "Company1", Description="This is default Company 1" },
                    new Company() { Name = "Company2", Description="This is default Company 2" },
                    new Company() { Name = "Company3", Description="This is default Company 3" },
                    new Company() { Name = "Company4", Description="This is default Company 4" },
                    new Company() { Name = "Company5", Description="This is default Company 5" }
                };

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                await context.SaveChangesAsync();

                //Get company Ids
                company1Id = context.Companies.FirstOrDefault(p => p.Name == "Company1")!.Id;
                company2Id = context.Companies.FirstOrDefault(p => p.Name == "Company2")!.Id;
                company3Id = context.Companies.FirstOrDefault(p => p.Name == "Company3")!.Id;
                company4Id = context.Companies.FirstOrDefault(p => p.Name == "Company4")!.Id;
                company5Id = context.Companies.FirstOrDefault(p => p.Name == "Company5")!.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Companies.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }


        public static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
        {
            //Seed Default Admin User
            var defaultUser = new BTUser
            {
                UserName = "btadmin1@bugtracker.com",
                Email = "btadmin1@bugtracker.com",
                FirstName = "Bill",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Admin));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Admin User
            defaultUser = new BTUser
            {
                UserName = "btadmin2@bugtracker.com",
                Email = "btadmin2@bugtracker.com",
                FirstName = "Steve",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Admin));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager1 User
            defaultUser = new BTUser
            {
                UserName = "ProjectManager1@bugtracker.com",
                Email = "ProjectManager1@bugtracker.com",
                FirstName = "John",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.ProjectManager));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new BTUser
            {
                UserName = "ProjectManager2@bugtracker.com",
                Email = "ProjectManager2@bugtracker.com",
                FirstName = "Jane",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.ProjectManager));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new BTUser
            {
                UserName = "Developer1@bugtracker.com",
                Email = "Developer1@bugtracker.com",
                FirstName = "Elon",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new BTUser
            {
                UserName = "Developer2@bugtracker.com",
                Email = "Developer2@bugtracker.com",
                FirstName = "James",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new BTUser
            {
                UserName = "Developer3@bugtracker.com",
                Email = "Developer3@bugtracker.com",
                FirstName = "Natasha",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer3 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new BTUser
            {
                UserName = "Developer4@bugtracker.com",
                Email = "Developer4@bugtracker.com",
                FirstName = "Carol",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer4 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new BTUser
            {
                UserName = "Developer5@bugtracker.com",
                Email = "Developer5@bugtracker.com",
                FirstName = "Tony",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer6 User
            defaultUser = new BTUser
            {
                UserName = "Developer6@bugtracker.com",
                Email = "Developer6@bugtracker.com",
                FirstName = "Bruce",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter1 User
            defaultUser = new BTUser
            {
                UserName = "Submitter1@bugtracker.com",
                Email = "Submitter1@bugtracker.com",
                FirstName = "Scott",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Submitter));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new BTUser
            {
                UserName = "Submitter2@bugtracker.com",
                Email = "Submitter2@bugtracker.com",
                FirstName = "Sue",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Submitter));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

        }

        public static async Task SeedDemoUsersAsync(UserManager<BTUser> userManager)
        {
            //Seed Demo Admin User
            var defaultUser = new BTUser
            {
                UserName = "demoadmin@bugtracker.com",
                Email = "demoadmin@bugtracker.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Admin));
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.DemoUser));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo ProjectManager User
            defaultUser = new BTUser
            {
                UserName = "demopm@bugtracker.com",
                Email = "demopm@bugtracker.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.ProjectManager));
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Developer User
            defaultUser = new BTUser
            {
                UserName = "demodev@bugtracker.com",
                Email = "demodev@bugtracker.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Developer));
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo Submitter User
            defaultUser = new BTUser
            {
                UserName = "demosub@bugtracker.com",
                Email = "demosub@bugtracker.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Submitter));
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Submitter User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Demo New User
            defaultUser = new BTUser
            {
                UserName = "demonew@bugtracker.com",
                Email = "demonew@bugtracker.com",
                FirstName = "Demo",
                LastName = "NewUser",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.Submitter));
                    await userManager.AddToRoleAsync(defaultUser, nameof(BTRole.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo New User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }


        public static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Models.ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                                                    new ProjectPriority() { Name = nameof(BTProjectPriority.Low) },
                                                    new ProjectPriority() { Name = nameof(BTProjectPriority.Medium) },
                                                    new ProjectPriority() { Name = nameof(BTProjectPriority.High) },
                                                    new ProjectPriority() { Name =nameof(BTProjectPriority.Urgent) },
                };

                var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
                await context.ProjectPriorities.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Project Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultProjectsAsync(ApplicationDbContext context)
        {

            //Get project priority Ids
            int priorityLow = context.ProjectPriorities.FirstOrDefault(p => p.Name == nameof(BTProjectPriority.Low))!.Id;
            int priorityMedium = context.ProjectPriorities.FirstOrDefault(p => p.Name == nameof(BTProjectPriority.Medium))!.Id;
            int priorityHigh = context.ProjectPriorities.FirstOrDefault(p => p.Name == nameof(BTProjectPriority.High))!.Id;
            int priorityUrgent = context.ProjectPriorities.FirstOrDefault(p => p.Name == nameof(BTProjectPriority.Urgent))!.Id;

            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Build a Personal Porfolio",
                         Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                         StartDate = DateTime.SpecifyKind(new DateTime(2021,8,20), DateTimeKind.Utc),
                         EndDate = DateTime.SpecifyKind(new DateTime(2021,8,20).AddMonths(1), DateTimeKind.Utc),
                         ProjectPriorityId = priorityLow
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Build a supplemental Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site.",
                         CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                         StartDate = DateTime.SpecifyKind(new DateTime(2021,8,20), DateTimeKind.Utc),
                         EndDate = DateTime.SpecifyKind(new DateTime(2021,8,20).AddMonths(4), DateTimeKind.Utc),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Build an Issue Tracking Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                         StartDate = DateTime.SpecifyKind(new DateTime(2021,8,20), DateTimeKind.Utc),
                         EndDate = DateTime.SpecifyKind(new DateTime(2021,8,20).AddMonths(6), DateTimeKind.Utc),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Build an Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user..",
                         CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                         StartDate = DateTime.SpecifyKind(new DateTime(2021,8,20), DateTimeKind.Utc),
                         EndDate = DateTime.SpecifyKind(new DateTime(2021,8,20).AddMonths(2), DateTimeKind.Utc),
                         ProjectPriorityId = priorityLow
                     },
                    new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Build a Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information.",
                         CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                         StartDate = DateTime.SpecifyKind(new DateTime(2021,8,20), DateTimeKind.Utc),
                         EndDate = DateTime.SpecifyKind(new DateTime(2021,8,20).AddMonths(3), DateTimeKind.Utc),
                         ProjectPriorityId = priorityHigh
                     }
                };

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                await context.SaveChangesAsync();


                //Get company Ids
                portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Build a Personal Porfolio")!.Id;
                blogId = context.Projects.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application")!.Id;
                bugTrackerId = context.Projects.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application")!.Id;
                movieId = context.Projects.FirstOrDefault(p => p.Name == "Build a Movie Information Web Application")!.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        // Add method after implemention of services
        //private static async Task AssignDefautProjectManagersAsync(ApplicationDbContext context)
        //{
        //    //Assign Movie Project Manager

        //    //Assign Portfolio Project Manager
        //}


        public static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>() {
                     new TicketType() { Name = nameof(BTTicketType.NewDevelopment) },      // Ticket involves development of a new, uncoded solution 
                     new TicketType() { Name = nameof(BTTicketType.WorkTask) },            // Ticket involves development of the specific ticket description 
                     new TicketType() { Name = nameof(BTTicketType.Defect)},               // Ticket involves unexpected development/maintenance on a previously designed feature/functionality
                     new TicketType() { Name = nameof(BTTicketType.ChangeRequest) },       // Ticket involves modification development of a previously designed feature/functionality
                     new TicketType() { Name = nameof(BTTicketType.Enhancement) },         // Ticket involves additional development on a previously designed feature or new functionality
                     new TicketType() { Name = nameof(BTTicketType.GeneralTask) }          // Ticket involves no software development but may involve tasks such as configuations, or hardware setup
                };

                var dbTicketTypes = context.TicketTypes.Select(c => c.Name).ToList();
                await context.TicketTypes.AddRangeAsync(ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Types.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>() {
                    new TicketStatus() { Name = nameof(BTTicketStatus.New) },                 // Newly Created ticket having never been assigned
                    new TicketStatus() { Name = nameof(BTTicketStatus.Development) },         // Ticket is assigned and currently being worked 
                    new TicketStatus() { Name = nameof(BTTicketStatus.Testing)  },            // Ticket is assigned and is currently being tested
                    new TicketStatus() { Name = nameof(BTTicketStatus.Resolved)  },           // Ticket remains assigned to the developer but work in now complete
                };

                var dbTicketStatuses = context.TicketStatuses.Select(c => c.Name).ToList();
                await context.TicketStatuses.AddRangeAsync(ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Statuses.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>() {
                                                    new TicketPriority() { Name = nameof(BTTicketPriority.Low)  },
                                                    new TicketPriority() { Name = nameof(BTTicketPriority.Medium) },
                                                    new TicketPriority() { Name = nameof(BTTicketPriority.High)},
                                                    new TicketPriority() { Name = nameof(BTTicketPriority.Urgent)},
                };

                var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
                await context.TicketPriorities.AddRangeAsync(ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }


        public static async Task SeedDefaultTicketsAsync(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            ////Get project Ids
            //portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Build a Personal Porfolio")!.Id;
            //blogId = context.Projects.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application")!.Id;
            //bugTrackerId = context.Projects.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application")!.Id;
            //movieId = context.Projects.FirstOrDefault(p => p.Name == "Build a Movie Information Web Application")!.Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketTypes.FirstOrDefault(p => p.Name == nameof(BTTicketType.NewDevelopment))!.Id;
            int typeWorkTask = context.TicketTypes.FirstOrDefault(p => p.Name == nameof(BTTicketType.WorkTask))!.Id;
            int typeDefect = context.TicketTypes.FirstOrDefault(p => p.Name == nameof(BTTicketType.Defect))!.Id;
            int typeEnhancement = context.TicketTypes.FirstOrDefault(p => p.Name == nameof(BTTicketType.Enhancement))!.Id;
            int typeChangeRequest = context.TicketTypes.FirstOrDefault(p => p.Name == nameof(BTTicketType.ChangeRequest))!.Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriorities.FirstOrDefault(p => p.Name == nameof(BTTicketPriority.Low))!.Id;
            int priorityMedium = context.TicketPriorities.FirstOrDefault(p => p.Name == nameof(BTTicketPriority.Medium))!.Id;
            int priorityHigh = context.TicketPriorities.FirstOrDefault(p => p.Name == nameof(BTTicketPriority.High))!.Id;
            int priorityUrgent = context.TicketPriorities.FirstOrDefault(p => p.Name == nameof(BTTicketPriority.Urgent))!.Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatuses.FirstOrDefault(p => p.Name == nameof(BTTicketStatus.New))!.Id;
            int statusDev = context.TicketStatuses.FirstOrDefault(p => p.Name == nameof(BTTicketStatus.Development))!.Id;
            int statusTest = context.TicketStatuses.FirstOrDefault(p => p.Name == nameof(BTTicketStatus.Testing))!.Id;
            int statusResolved = context.TicketStatuses.FirstOrDefault(p => p.Name == nameof(BTTicketStatus.Resolved))!.Id;

            //Get admin Ids
            string company1AdminId = (await userManager.FindByEmailAsync("btadmin1@bugtracker.com"))!.Id;
            string company2AdminId = (await userManager.FindByEmailAsync("btadmin2@bugtracker.com"))!.Id;

            try
            {
                IList<Ticket> tickets = new List<Ticket>() {
                    //PORTFOLIO
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 1", Description = "Ticket details for portfolio ticket 1", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 2", Description = "Ticket details for portfolio ticket 2", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 3", Description = "Ticket details for portfolio ticket 3", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 4", Description = "Ticket details for portfolio ticket 4", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 5", Description = "Ticket details for portfolio ticket 5", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeDefect},
                    //BLOG
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 2", Description = "Ticket details for blog ticket 2", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 3", Description = "Ticket details for blog ticket 3", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 4", Description = "Ticket details for blog ticket 4", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 5", Description = "Ticket details for blog ticket 5", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 9", Description = "Ticket details for blog ticket 9", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew,  TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 10", Description = "Ticket details for blog ticket 10", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 11", Description = "Ticket details for blog ticket 11", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 12", Description = "Ticket details for blog ticket 12", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew,  TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 13", Description = "Ticket details for blog ticket 13", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 14", Description = "Ticket details for blog ticket 14", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev,  TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 15", Description = "Ticket details for blog ticket 15", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew,  TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 16", Description = "Ticket details for blog ticket 16", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company2AdminId, Title = "Blog Ticket 17", Description = "Ticket details for blog ticket 17", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                    //BUGTRACKER                                                                                                                         
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 1", Description = "Ticket details for bug tracker ticket 1", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 2", Description = "Ticket details for bug tracker ticket 2", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 3", Description = "Ticket details for bug tracker ticket 3", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 4", Description = "Ticket details for bug tracker ticket 4", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 5", Description = "Ticket details for bug tracker ticket 5", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 6", Description = "Ticket details for bug tracker ticket 6", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 7", Description = "Ticket details for bug tracker ticket 7", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 8", Description = "Ticket details for bug tracker ticket 8", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 9", Description = "Ticket details for bug tracker ticket 9", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 10", Description = "Ticket details for bug tracker 10", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 11", Description = "Ticket details for bug tracker 11", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 12", Description = "Ticket details for bug tracker 12", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 13", Description = "Ticket details for bug tracker 13", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 14", Description = "Ticket details for bug tracker 14", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 15", Description = "Ticket details for bug tracker 15", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 16", Description = "Ticket details for bug tracker 16", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 17", Description = "Ticket details for bug tracker 17", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 18", Description = "Ticket details for bug tracker 18", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 19", Description = "Ticket details for bug tracker 19", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 20", Description = "Ticket details for bug tracker 20", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 21", Description = "Ticket details for bug tracker 21", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 22", Description = "Ticket details for bug tracker 22", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 23", Description = "Ticket details for bug tracker 23", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 24", Description = "Ticket details for bug tracker 24", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 25", Description = "Ticket details for bug tracker 25", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 26", Description = "Ticket details for bug tracker 26", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 27", Description = "Ticket details for bug tracker 27", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 28", Description = "Ticket details for bug tracker 28", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 29", Description = "Ticket details for bug tracker 29", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Bug Tracker Ticket 30", Description = "Ticket details for bug tracker 30", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = bugTrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    //MOVIE
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 1", Description = "Ticket details for movie ticket 1", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 2", Description = "Ticket details for movie ticket 2", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 3", Description = "Ticket details for movie ticket 3", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 4", Description = "Ticket details for movie ticket 4", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 5", Description = "Ticket details for movie ticket 5", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 6", Description = "Ticket details for movie ticket 6", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 7", Description = "Ticket details for movie ticket 7", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 8", Description = "Ticket details for movie ticket 8", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 9", Description = "Ticket details for movie ticket 9", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityLow, TicketStatusId = statusNew,  TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 10", Description = "Ticket details for movie ticket 10", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew, TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 11", Description = "Ticket details for movie ticket 11", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 12", Description = "Ticket details for movie ticket 12", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew,  TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 13", Description = "Ticket details for movie ticket 13", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 14", Description = "Ticket details for movie ticket 14", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev,  TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 15", Description = "Ticket details for movie ticket 15", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew,  TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 16", Description = "Ticket details for movie ticket 16", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 17", Description = "Ticket details for movie ticket 17", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 18", Description = "Ticket details for movie ticket 18", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev,  TicketTypeId = typeEnhancement},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 19", Description = "Ticket details for movie ticket 19", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew,  TicketTypeId = typeChangeRequest},
                    new Ticket() {OwnerUserId = company1AdminId, Title = "Movie Ticket 20", Description = "Ticket details for movie ticket 20", CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc), ProjectId = movieId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},

                };


                var dbTickets = context.Tickets.Select(c => c.Title).ToList();
                await context.Tickets.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultNotificationTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<NotificationType> notificationTypes = new List<NotificationType>() {
                     new NotificationType() { Name = BTNotificationType.Project.ToString() },
                     new NotificationType() { Name = BTNotificationType.Ticket.ToString() }
                };

                var dbNotificationTypes = context.NotificationTypes.Select(c => c.Name).ToList();
                await context.NotificationTypes.AddRangeAsync(notificationTypes.Where(c => !dbNotificationTypes.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Notification Types.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
    }
}
