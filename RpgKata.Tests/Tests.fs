module RpgKataTests

open Character
open Xunit

let private baseCharacter =
    { Health = 1000
      Level = 1
      Status = Alive }

let private deadCharacter = { Health = 0; Level = 1; Status = Dead }

[<Fact>]
let ``Kill character if damage is higher than remaining health`` () =
    let expected = deadCharacter
    let actual = DamageChar baseCharacter 2000
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't kill character if damage is less than remaining health`` () =
    let expected =
        { Health = 0
          Level = 1
          Status = Alive }

    let actual = DamageChar baseCharacter 1000
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he's dead`` () =
    let expected = deadCharacter
    let actual = HealChar deadCharacter 200
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he is at max health`` () =
    let expected = baseCharacter
    let actual = HealChar baseCharacter 200
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't overheal character`` () =
    let character =
        { Health = 999
          Level = 1
          Status = Alive }

    let expected = baseCharacter
    let actual = HealChar character 200
    Assert.Equal(expected, actual)

[<Fact>]
let ``Heal character if he is alive and at less than max health`` () =
    let character =
        { Health = 800
          Level = 1
          Status = Alive }

    let expected = { character with Health = 900 }
    let actual = HealChar character 100
    Assert.Equal(expected, actual)
