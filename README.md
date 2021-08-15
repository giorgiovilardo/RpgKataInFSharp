# RpgKata in F#

Reference: https://github.com/ardalis/kata-catalog/blob/main/katas/RPG%20Combat.md

## Notes

### Iteration 1 

#### Before

"Looks easy enough if I ignore state/identity"

#### After

* I definitely need more practice on how to structure F# programs
* I wanted to use function composition operator `>>` to model complex behaviour but was bitten in the ass by the type signatures
  * Maybe if I use a factory that returns back a partially applied function after changing the order of the parameters...ok no jk

### Iteration 2

#### Before

* Oof, identity again
  * Maybe I will experiment with a `NamedCharacter` type that has a Name and a `Character` as data, to not violate OCP
* Oof, more parameters

#### Refactor step

Actually changing the order of the parameters in the damage/heal functions
was a good idea; now I can preload damage and have a composable `Character -> Character` function.

#### After

* Meh, it's breaking up really quickly
* Logic is convoluted and starts to become unreadable?

### Iteration 3

#### Before

* Requisites looks horrendously declared
* I think it's some kind of ranged can damage ranged + melee, melee only melee?
* I have to modify again the record?

#### Refactor step

* Yes, "refactor"
* Tried to rationalize everything
* ClassedCharacter type then needs generic functions? should character be a DU?

#### After

* kinda easy to implement the range check
* functions are starting to have too many responsibilities and being too big
  * I don't get how to correctly chain them specially when a bool check is required to proceed and I don't want to use `failwith`
    * probably validation via `Some/None` type? Or a `ValidatedCharacter` type?
  * Then I need to carry `Some/None` or `Ok/Error` through the whole pipe

### Iteration 4

#### Before

* I keep delaying making Characters a DU, but now looks like it's the time
  * I need to implement some sort of a master dispatcher functions that then sends you to the correct damage check?
  * Rest is easy-ish just have a return new record with different faction

#### DU BRANCH

* Wtf i do? My funcs are for Character which should be able to iterate a la inheritance over all versions of Character? Why I have to rematch and unwrap?
