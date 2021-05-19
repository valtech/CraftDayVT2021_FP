# FP and ROP exercises for Craft Day VT2021 
## Layout
The repository consists of seven different parts divided into two major assignments.

### First, the three projects consisting of the application used as an example:
* **CraftDay.ToDo.CSharp** 
  * A (mock) TODO API-project written in OOP-style (used as comparison and reference)
* **CraftDay.ToDo.CSharpRop** 
  * The same API-project written in ROP-style (used to exemplify ROP-style projects)
* **CraftDay.ToDo.FSharpRop** 
  * The same API-project written in ROP-style using a functional language (used to showcase how FP supports ROP and adds extra guarantees)

### Two test beds
* **CraftDay.ToDo.Unit.Test**
  * Shared unit tests for the three application implemetations
* **CraftDay.ToDo.Integration.Test**
  * Shared integration tests for the three application implemetations

### Some shared objects (to reduce duplication)
* **CraftDay.ToDo.CSharp**
  * DTOs etc..
* **CraftDay.ToDo.Util.Test**
  * Shared resources between Unit and Integration tests
