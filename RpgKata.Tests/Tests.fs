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

let private baseTarget =
    { Name = "Target"
      Stats = baseAliveStats }

let private deadCharacter =
    { Name = "Target"
      Stats = baseDeadStats }

let private damageFor2000 sourceChar destChar =
    damageCharacter sourceChar destChar 2000

let private damageFor1000 sourceChar destChar =
    damageCharacter sourceChar destChar 1000

let private healFor200 sourceChar destChar = healCharacter sourceChar destChar 200
let private healFor100 sourceChar destChar = healCharacter sourceChar destChar 100

[<Fact>]
let ``Kill character if damage is higher than remaining health`` () =
    let expected = deadCharacter
    let actual = damageFor2000 baseCharacter baseTarget
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't kill character if damage is less than remaining health`` () =
    let expected =
        { Name = "Target"
          Stats =
              { Health = 0
                Level = 1
                Status = Alive } }

    let actual = damageFor1000 baseCharacter baseTarget
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he's dead`` () =
    let expected = deadCharacter
    let actual = healFor200 deadCharacter deadCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't heal character if he is at max health`` () =
    let expected = baseCharacter
    let actual = healFor200 baseCharacter baseCharacter
    Assert.Equal(expected, actual)

[<Fact>]
let ``Don't overheal character`` () =
    let character =
        { baseCharacter with
              Stats =
                  { baseCharacter.Stats with
                        Health = 999 } }

    let expected = baseCharacter
    let actual = healFor200 character character
    Assert.Equal(expected, actual)

[<Fact>]
let ``Heal character if he is alive and at less than max health`` () =
    let character =
        { baseCharacter with
              Stats =
                  { baseCharacter.Stats with
                        Health = 800 } }

    let expected =
        { character with
              Stats = { character.Stats with Health = 900 } }

    let actual = healFor100 character character
    Assert.Equal(expected, actual)

[<Fact>]
let ``If level is higher on target, damage is reduced`` () =
    let sourceCharacter = baseCharacter

    let destCharacter =
        { baseTarget with
              Stats = { baseTarget.Stats with Level = 10 } }

    Assert.Equal(200 / 2, normalizeDamage sourceCharacter.Stats destCharacter.Stats 200)

[<Fact>]
let ``If level is lower on target, damage is doubled`` () =
    let sourceCharacter =
        { baseCharacter with
              Stats = { baseCharacter.Stats with Level = 10 } }

    let destCharacter = baseCharacter
    Assert.Equal(200 * 2, normalizeDamage sourceCharacter.Stats destCharacter.Stats 200)

[<Fact>]
let ``A character can't damage itself`` () =
    Assert.Equal(baseCharacter, damageFor2000 baseCharacter baseCharacter)

[<Fact>]
let ``Can only heal itself so a Character cannot heal another Character`` () =
    let damagedChar =
        { Name = "Target"
          Stats =
              { Health = 390
                Level = 1
                Status = Alive } }

    Assert.Equal(damagedChar, healFor200 baseCharacter damagedChar)

[<Fact>]
let ``Can only heal itself so anotherChar can heal anotherChar`` () =
    let damagedChar =
        { Name = "Target"
          Stats =
              { Health = 390
                Level = 1
                Status = Alive } }

    let expected =
        { damagedChar with
              Stats = { damagedChar.Stats with Health = 590 } }

    Assert.Equal(expected, healFor200 damagedChar damagedChar)
