
## starting app

```bash
# start dotnet app
$ dotnet run || dotnet run watch

# start angular side
$ ng serve --host 0.0.0.0

```


#### used to be to call or shortcut -->

``` control + ship + P => nuget Microsoft.EntityFrameworkCore.Sqlite ```

# Initialize new migration  
```bash

$ dotnet ef migrations add InitialCreate 
                        -  AddedUserEntity
                        -  ExtendedUserClass
                        
# after models and DataContext for migration
$ dotnet ef database update


# show list of migrations
$ dotnet ef migrations list

# remove latest migration 
$ dotnet ef migrations remove

# Drop database 
$ dotnet ef database drop
```

# short cuts
```bash

# constructor 
 ctor
#
 prop =? inside model

```


#### Skeleton 1
1. make HTTP request from Angular to fetch data from API 
2. Bind data from the API to the HTML to display it on z page 

#### Skeleton 2
1. creating Authentication Controller
2. Data Transfer Objects
3. Token Authentication **:** *to not keep login for user*
    - https://jwt.io/
4. Authentication MiddleWare **:** *to protect controllers && method inside*

#### Skeleton 2
2. design client side using bootstrap 
2. Angular template forms and services
4. Angular input and output properties
3. display user info on page if Authenticated 

#### Skeleton 3
- Handling errors from the api and handle error on the client

#### Skeleton 4
- Aletifyjs :: notify user for something happens
- Angular JWT :: send up jwt for every single req make to api / check if token has expired or exist
- NGX Bootstrap :: alternative angular similar to B.js (to not use "Jquery" of course!)

#### Skeleton 5
- Setting up routing and using RouterLinkActive
- Using Routing code and protecting the Routes 'multiple routes' / [ Guard ]
*( ng g guard auth --spec=false )

#### Skeleton 5
1. Extending the User Class => store more data
    - dotnet ef migrations add ExtendedUserClass
2. migration and cascade Delete => for relationship data`s 
    - to make sure photos associated with the user are also deleted
3. Seeding data into db
4. Using Automapper => intits into dto and back again automatically into controller rather than doing mappings manually

#### Skeleton 6
1. retrieving users to the client from the API
2. create Member Cards (for base info of users)
3. Adding a Detailed view of the users
4. Route resolves (retrieve data before the route activated 'Access the data when the route is being loaded' ) 
5. Adding Photos(Portfolio) gallery

#### Skeleton 7
1. canDeactive Route Guard - prevent user accidentally clicking different nav if they making changes to form if they have unsaved changes     
2. @HostListener decoration  # prevent user losing there changes if they try quit the browser
3. @ViewChild access formComponent 
4. Persist changes to the APi

#### Skeleton 8
1. Adding file uploader *() to SPA (NG2 file-upload)
3. setting Main photo
4. Any to Any component communication
5. add single, multiple photo and delete single photo (Filter, Splice) 
``` git update-index --assume-unchanged PpsCode.API/appsettings.json ```

#### Skeleton 9
1. Get More info from user when register
2. (UI)* Reactive Form /Custom Validation/ Validation Feedback When user Register
3. Action Filters (display user timeAgo*(Last Active) when user make an action inside the application) && dates SPA pipes