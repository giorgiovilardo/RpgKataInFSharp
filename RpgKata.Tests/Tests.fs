module RpgKataTests

open Character
open Xunit

let private baseCharacter =
    { Health = 1000
      Level = 1
      Status = Alive }

let private deadCharacter = { Health = 0; Level = 1; Status = Dead }
let private DamageFor2000 = DamageChar 2000
let private DamageFor1000 = DamageChar 1000
let private HealFor200 = HealChar 200
let private HealFor100 = HealChar 100

[<Fact>]
let ``Kill character if damage is higher than remaining health`` () =
    let expected = deadCharacter
    let actual = DamageFor2000 baseCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't kill character if damage is less than remaining health`` () =
    let expected =
        { Health = 0
          Level = 1
          Status = Alive }

    let actual = DamageFor1000 baseCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he's dead`` () =
    let expected = deadCharacter
    let actual = HealFor200 deadCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he is at max health`` () =
    let expected = baseCharacter
    let actual = HealFor200 baseCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't overheal character`` () =
    let character =
        { Health = 999
          Level = 1
          Status = Alive }

    let expected = baseCharacter
    let actual = HealFor200 character
    Assert.Equal(expected, actual)

[<Fact>]
let ``Heal character if he is alive and at less than max health`` () =
    let character =
        { Health = 800
          Level = 1
          Status = Alive }

    let expected = { character with Health = 900 }
    let actual = HealFor100 character
    Assert.Equal(expected, actual)
