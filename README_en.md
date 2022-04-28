# NaurokApiClient

[EN](README_en.md) | [RU](README.md)

## What is NaurokApiClient?
In Ukraine, there is a popular site https://naurok.com.ua, designed for testing students. It has a non-public, undocumented API.
This class library is an API client for the site and allows you to interact with most of its functions.

The uniqueness of the library is detailed documentation in Russian (via *summary*) of each method and class,
from which you can find out all the limitations and possible causes of errors.

## What functionality is available?
At the moment, the library supports the features of unauthorized users and teachers.
Since authorization on the site is carried out using *reCaptcha*, authorization in the library is implemented through *cookie*-value '*PHPSESSID*'.

Implemented functionality of *unauthorized user*:
* Getting a testing session
* Completion of the testing session
* Saving the answer to the testing question
* Obtaining test flashcards (if you have access from the teacher)
* Getting cards and the best result of the game for matching questions with answers (if you have access from the teacher)
* Saving the result of the game to match questions with answers (if you have access from the teacher)

Implemented functionality *user-teacher*:
* All functionality of an unauthorized user
* Getting information about a question
* Obtaining test flashcards (with basic certification)
* Getting cards and the best result of the game for matching questions with answers (if you have basic certification)
* Saving the result of the game for matching questions with answers (if you have basic certification)
* Receiving your test document
* Searching test documents with similar questions
* Receiving a short and complete excel-report on the passage of homework

A fully working *RealTime* client has also been implemented to interact with real-time tests.

## What is planned to add?
* Checking the value of '*PHPSESSID*' for teacher user authorization
* Checking if a teacher user has a basic certificate
* Expansion of functionality
* Adding *Unit* tests to replace the console project
* Adding online documentation (wiki) to *github* in this repository
* Creation of a full-fledged client - a mobile application

## Documentation
The documentation is implemented in the code via *summary* (completely), as well as in the repository [here] (https://github.com/IhorKuzmichov/KuzCode.NaurokApiClient/wiki) (not completely).

## Notes
The project uses *user-secrets*. The *secret.json* file looks like this:
```json
{
  "teacherPhpSessionId": "PHPSESSID of teacher...",
  "sessionId": 0,
  "documentId": 0,
  "userAgent": "User-Agent Header..."
}
```

## License
This project is covered by the *MIT* license.

