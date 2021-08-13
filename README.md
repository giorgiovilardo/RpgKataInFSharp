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
