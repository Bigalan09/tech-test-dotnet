### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around 1 to 3 hours to complete the exercise.

---
# Changelog

_**NOTE**: These changes are all based on assuptions. In a real world scenario, I'd be asking questions around the functional and non functional requirements._

- [x] Guard Clause
  - [ ] Observation 2 - Validator classes and domain models.
- [x] Data store
- [x] MakePaymentResult static factory
- [x] AllowedPaymentScheme enum
- [x] Payment scheme policy checks
- [ ] Unit of work
- [ ] Idempotency
- [ ] Outbox pattern
- [ ] Ledger
- [ ] Observability
- [ ] Concurrency issues

## Guard Clause
The `MakePaymentResult MakePayment(MakePaymentRequest request)` needs guard clauses and input validation.
We can exit early if the guard clauses and input validation fails.
Observation 2: I'd also make use of domain models and validators within the domain model, decoupling the validation from the payment method and simplifying tests.

## Data store
The `MakePaymentResult MakePayment(MakePaymentRequest request)` is doing a lot, it's not the role of the payment service to determine which datastore to use. 
Refactor to inject a IDataStore and configure it via dependency injection registration. _NOTE: Ideally the backup database would be the concern for infrastructure,
introducing a data persistence replication and a read only always available readonly connection to the replicas via a load balancer._ 

## MakePaymentResult static factory
To minimise the risk of misuse, add static methods for the creation of MakePaymentResult.
Add immutable fields, making it safer to use.

## AllowedPaymentScheme enum
Add a `[Flag]` attribute and a `None = 0` value. Helping the scheme checks be more consistent.

## Payment scheme policy checks
Instead of a switch statement, we can introduce an `IPaymentPolicy` per scheme, giving us the ability to individually test scheme policies,
future proofing for additional schemes.

## Unit of work
We should wrap each database write within a single transaction, ensuring safety on failure. We can rollback a transaction when something fails.

## Idempotency
To prevent procesing duplicate payment requests, adding an idempotency key which is saved in the database, along with the resulting outcome of the method (success or rejection) means if we process a message more than once, we can return the stored result and not process the payment again.

## Outbox pattern
The outbox pattern allows us to write outgoing events to the database (inside the unit of work), so that they can be processed on successful payment. The record would rollback if a failure occurs meaning we dont have inconsistent states in external systems.

## Ledger
Introducing an append only ledger for successful and rejected payments would help with auditablity and reconcilliation.
We could also derive the account balance from the ledger record.

## Observability
Add structured logs, correlation id, idempotency keys, rejection reasons, etc for improved observability and operational safety.
(optional) Mask personally identifiable information (PPI) in logs.

## Concurrency issues
There is potential that 2 payments happen at the same time for the same account,
we need to make sure writes are atomic. Possible implementation patterns is to include a row version. When reading the balance for example,
take note of the RowVersion, when updating the balance, pass the RowVersion into the update statement, if zero rows were updated, either reject or retry.