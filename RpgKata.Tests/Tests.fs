module RpgKataTests

open Character
open Xunit


let private baseAliveStats =
    { Health = 1000
      Level = 1
      Status = Alive }

let private baseDeadStats = { Health = 0; Level = 1; Status = Dead }

let private baseCharacter =
    { Name = "Base"
      Stats = baseAliveStats }

let private deadCharacter = { Name = "Dead"; Stats = baseDeadStats }

//let private DamageFor2000 = DamageChar 2000
//let private DamageFor1000 = DamageChar 1000
//let private HealFor200 = HealChar 200
//let private HealFor100 = HealChar 100

[<Fact>]
let ``Kill character if damage is higher than remaining health`` () =
    let expected = deadCharacter
    let actual = DamageFor2000 baseCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't kill character if damage is less than remaining health`` () =
    let expected =
        { Name = ""
          Health = 0
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
        { Name = ""
          Health = 999
          Level = 1
          Status = Alive }

    let expected = baseCharacter
    let actual = HealFor200 character
    Assert.Equal(expected, actual)

[<Fact>]
let ``Heal character if he is alive and at less than max health`` () =
    let character =
        { Name = ""
          Health = 800
          Level = 1
          Status = Alive }

    let expected = { character with Health = 900 }
    let actual = HealFor100 character
    Assert.Equal(expected, actual)

[<Fact>]
let ``If level is higher on target, damage is reduced`` () =
    let sourceCharacter = baseCharacter

    let destCharacter =
        { Name = ""
          Health = 1000
          Level = 40
          Status = Alive }

    Assert.Equal(100, normalizeDamage sourceCharacter destCharacter 200)

[<Fact>]
let ``If level is lower on target, damage is reduced`` () =
    let sourceCharacter =
        { Name = ""
          Health = 1000
          Level = 40
          Status = Alive }

    let destCharacter = baseCharacter

    Assert.Equal(400, normalizeDamage sourceCharacter destCharacter 200)

[<Fact>]
let ``Can't damage itself`` () =
    let sourceCharacter = baseCharacter
    let destCharacter = baseCharacter
    let expected = baseCharacter
    let actual = Damage sourceCharacter 200 destCharacter

    Assert.Equal(expected, actual)

[<Fact>]
let ``Complete round with Damage`` () =
    let sourceCharacter = baseCharacter

    let destCharacter =
        { Name = "Mostro"
          Health = 1000
          Level = 1
          Status = Alive }

    let expected =
        { Name = "Mostro"
          Health = 800
          Level = 1
          Status = Alive }

    Assert.Equal(expected, Damage sourceCharacter 200 destCharacter)

[<Fact>]
let ``Can only heal itself so baseChar cannot heal anotherChar`` () =
    let anotherChar =
        { Name = "Altro"
          Health = 390
          Level = 1
          Status = Alive }

    Assert.Equal(anotherChar, Heal baseCharacter 400 anotherChar)

[<Fact>]
let ``Can only heal itself so anotherChar can heal anotherChar`` () =
    let anotherChar =
        { Name = "Altro"
          Health = 390
          Level = 1
          Status = Alive }

    let anotherCharHealed =
        { Name = "Altro"
          Health = 790
          Level = 1
          Status = Alive }

    Assert.Equal(anotherCharHealed, Heal anotherChar 400 anotherChar)
