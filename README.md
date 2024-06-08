Logic and things to consider at running
=======================
https://learn.microsoft.com/en-us/answers/questions/1287896/getting-an-error-add-migration-the-term-add-migrat

the project was build in .Net Core 6 and it also includes Entity Framework 6 Structures in order to have a database to handle a basic POS Configuration and a ShoppingCart session,therefore the use of a database in SQL is required and is handle in the class EFCashMasterDbContext.cs ,some migrations files were modified manually in order to have foreing keys as needed and the use of FirstCode tools as the package Manager Console is precise to run the following instruction :
```ruby
#this code must run on PACKAGE MANAGER CONSOLE
#TOOLS->NUGET PACKAGE MANAGER-> PACKAGE MANAGER CONSOLE
update-database
```
the POS it consist in the following Areas:
| Class | Description |
| --- | --- |
| POS | you can create a pos by Country in here with the method CreateNewPOS if you want to use an existing one just using the constructor would be enough,something to consider is than the POS was build thinking in handle BaseCurrency depending of the country but that doesn't mean you can not pay with a foreing currency,you can do it but the exchange will be payback in the BaseCurrency of the POS |
| CurrencyType | is a catalog of the currencies that a POS is able to Handle for paybacks calculations |
| Parity | catalog of the Raw  value of foreing currencies based in the BaseCurrency of the POS |
| ParityApraisal | configuration to let us know how should we do the convertion from a foreing Currency to the BaseCurrency of the POS |
| Product | Products Catalog Segmented by POS Location |
| Price | Catalog of Prices by date for every product |
| Stock | Configuration of Acountabily of Entries and Exits of a product |
| ExchangeType | Catalog of exchangeTypes per Denomination(Identify Bills and Coins) |
| ShoppingCart | it saves the general Statistics of the sale until is Closed / Commited |
| ShoppingCartDetail | it saves the relation between shopping cart and the Quantity Ordered by a Product |
| Payment | A record of payment for the full sale |
| PaymentCurrencyDenominations | it saves a list of the ExchangeType denominations that the client use in order to pay and it also saves the denominations that are going to be payback to a client for the sale |

something to know is that the Main Function in Program.cs has configure a Base structure of how every element should be use 
the UnitTesting was build on XUnit and it Use a Memory enviroment to emulate the database and the behaviour of Entity Framework




CashMaster  Challenge
=======================
You are a software consultant for CASH Masters, a company that sells point-of-sale (POS) electronic cash registers.  CASH Masters would like to rewrite their POS system from scratch and has the requirement below that they’d like you to implement. Provide a complete working solution of how you would implement this. Pay attention to all function and non-function requirements and treat this as if you were coding as a member of the CASH Master team.

Functional Requirements
=======================
Today’s young cashiers are increasingly unable to return the correct amount of change.  As a result, we would like our POS system to calculate the correct change and display the optimum (i.e. minimum) number of bills and coins to return to the customer. 
  
Your task is to write a routine that takes two inputs:
•	Price of the item(s) being purchased
•	All bills and coins provided by the customer to pay for the item(s)

The routine should return the smallest number of bills and coins equal to the change due.

Note: the customer may not submit an optimal number of bills and coins. For example, if the price is $5.25, they might provide one $5 bill and one $1 bill, or, they could provide one $5 bill and ten dimes ($0.10).  The only expectation is that the total of what they provide is greater that or equal to the price of the item they’re purchasing.  Your function should verify this assumption.

Since other engineers will be using your new function, recommend an appropriate data structure for the bills and coins. This structure should be used for the input parameter and for the returned value.  Additionally, this system will be sold around the world.  Each country will have its own denomination of bills and coins. For example, here are denomination lists for two countries where our POS might be sold:

•	US: 0.01, 0.05, 0.10, 0.25, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00<br/>
•	Mexico: 0.05, 0.10, 0.20, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00

You are not required to physically distinguish whether the values are bills or coins, only their numeric values.

When a POS terminal is sold and installed in a given country, its currency setting will be set only once as a global configuration.  It should be clear to any engineers that will be calling your routine in the future how to implement this.  Your routine should not take this as input with each call.
    
Non-Functional Requirements
===========================
•	Write a C# .NET console app that demonstrates your working routine<br/> 
•	Provide comments to help future engineers use, and extend your function<br/>
•	Unit tests should provide complete coverage of all key aspects of your function<br/>
•	Common Objective Oriented principles should be applied<br/>
•	Your routine should perform as fast as possible<br/>
•	You need to have robust error handling with clearly documented exception definitions for callers of your routine<br/>

 
