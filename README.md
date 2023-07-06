# Patients
Query and Console Application Kata

1. Run TestDBSetup.sql to setup the test database.
2. Write a query to return a list of all members and any of their corresponding categories:
  a. Please include the following fields: Member ID, First Name, Last Name, Most Severe Diagnosis ID, Most Severe Diagnosis Description, Category ID, Category Description, Category Score and Is Most Severe Category.
  b. Most Severe Diagnosis ID and Most Severe Diagnosis Description should be the diagnosis with the smallest Diagnosis ID for each Member/Category pair. Set to NULL for members without diagnosis.
  c. Is Most Severe Category – Set to 1 for the smallest Category ID for a Member, all other categories for that member should be set to 0. For members without categories, set to 1.
  d. This query should return one result for each Member/Category pair. Members without categories should be included as well.
4. Extra Credit: Write a C# Console Application that prompts for a Member ID and displays the results of query #3 for that Member
