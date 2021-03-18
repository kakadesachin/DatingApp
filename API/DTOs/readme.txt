DTOs stands for Data Transfer Objects.

whenever we want to share values through API's
its not necessary that we have to use the Entity Objects.
Because we might not share all the properties.
to make the http request small, we can create Classes to just do that small task.

example in RegisterDto

we want users to post values in form of Objects with properties like
username and password.
we cannot expose our AppUser schema.

so create RegisterDto class to handle register type.
similarly with LoginDto