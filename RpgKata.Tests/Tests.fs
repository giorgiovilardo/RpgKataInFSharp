module RpgKataTests

open Character
open Xunit


let private baseAliveStats =
    { Health = 1000
      Level = 1
      Status = Alive
      Range = 2 }

let private baseAliveRangedStats =
    { Health = 1000
      Level = 1
      Status = Alive
      Range = 30 }

let private baseDeadStats =
    { Health = 0
      Level = 1
      Status = Dead
      Range = 2 }

let private baseDeadRangedStats =
    { Health = 0
      Level = 1
      Status = Dead
      Range = 30 }

let private baseCharacter =
    { Name = "Base"
      Faction = None
      Stats = baseAliveStats }

let private baseTarget =
    { Name = "Target"
      Faction = None
      Stats = baseAliveStats }

let private baseRangedCharacter =
    { Name = "Base"
      Faction = None
      Stats = baseAliveRangedStats }

let private baseRangedTarget =
    { Name = "Target"
      Faction = None
      Stats = baseAliveRangedStats }

let private deadCharacter =
    { Name = "Target"
      Faction = None
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
          Faction = None
          Stats =
              { Health = 0
                Level = 1
                Status = Alive
                Range = 2 } }

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
          Faction = None
          Stats =
              { Health = 390
                Level = 1
                Status = Alive
                Range = 2 } }

    Assert.Equal(damagedChar, healFor200 baseCharacter damagedChar)

[<Fact>]
let ``Can only heal itself so anotherChar can heal anotherChar`` () =
    let damagedChar =
        { Name = "Target"
          Faction = None
          Stats =
              { Health = 390
                Level = 1
                Status = Alive
                Range = 2 } }

    let expected =
        { damagedChar with
              Stats = { damagedChar.Stats with Health = 590 } }

    Assert.Equal(expected, healFor200 damagedChar damagedChar)

[<Fact>]
let ``A Character can only damage a character in range`` () =
    // Truth table of who damages who:
    //       | R | M |
    // Ranged| Y | Y |
    // Melee | N | Y |
    let damagedMeleeChar =
        { Name = "Target"
          Faction = None
          Stats =
              { Health = 390
                Level = 1
                Status = Alive
                Range = 2 } }

    let damagedRangedChar =
        { Name = "Target"
          Faction = None
          Stats =
              { Health = 390
                Level = 1
                Status = Alive
                Range = 30 } }
    // Ranged damages ranged
    Assert.Equal(
        damagedRangedChar,
        damageCharacter
            baseRangedCharacter
            { baseRangedCharacter with
                  Name = "Target" }
            610
    )
    // Ranged damages melee
    Assert.Equal(damagedMeleeChar, damageCharacter baseRangedCharacter baseTarget 610)
    // Melee damages melee
    Assert.Equal(damagedMeleeChar, damageCharacter baseCharacter baseTarget 610)
    // Melee can't damage ranged
    Assert.Equal(baseRangedCharacter, damageCharacter baseCharacter baseRangedCharacter 610)

[<Fact>]
let ``A character can join a faction`` () =
    let joinedChar = joinFaction baseCharacter "Orcs"
    Assert.Equal(Some [ "Orcs" ], joinedChar.Faction)

[<Fact>]
let ``A character can join more than one faction`` () =
    let joinedChar = joinFaction baseCharacter "Orcs"
    let joinedChar = joinFaction joinedChar "Goblins"
    Assert.Equal(Some [ "Goblins"; "Orcs" ], joinedChar.Faction)

[<Fact>]
let ``A character can leave a faction`` () =
    let joinedChar = joinFaction baseCharacter "Orcs"
    let joinedChar = joinFaction joinedChar "Goblins"
    let joinedChar = leaveFaction joinedChar "Orcs"
    Assert.Equal(Some [ "Goblins" ], joinedChar.Faction)

[<Fact>]
let ``A character who leaves last faction goes back to None`` () =
    let joinedChar = joinFaction baseCharacter "Orcs"
    let joinedChar = leaveFaction joinedChar "Orcs"
    Assert.Equal(None, joinedChar.Faction)

[<Fact>]
let ``An Ally cannot damage another Ally`` () =
    let baseCharacter =
        { baseCharacter with
              Faction = Some [ "Goblins" ] }

    let expected =
        { baseTarget with
              Faction = Some [ "Goblins" ] }

    Assert.Equal(expected, damageFor2000 baseCharacter expected)

[<Fact>]
let ``An Ally can heal another Ally`` () =
    let baseCharacter =
        { baseCharacter with
              Faction = Some [ "Goblins" ] }

    let damagedCharacter =
        { baseTarget with
              Faction = Some [ "Goblins" ]
              Stats = { baseTarget.Stats with Health = 400 } }

    let expected =
        { damagedCharacter with
              Stats =
                  { damagedCharacter.Stats with
                        Health = 600 } }

    Assert.Equal(expected, healFor200 baseCharacter damagedCharacter)

[<Fact>]
let ``Can damage a Prop via precise function`` () =
    let baseProp =
        { Name = "Tree"
          Health = 2000
          PropStatus = Intact }

    let expected =
        { Name = "Tree"
          Health = 1
          PropStatus = Intact }

    Assert.Equal(expected, damageProp baseProp 1999)

[<Fact>]
let ``Can kill a Prop via precise function`` () =
    let baseProp =
        { Name = "Tree"
          Health = 2000
          PropStatus = Intact }

    let expected =
        { Name = "Tree"
          Health = 0
          PropStatus = Destroyed }

    Assert.Equal(expected, damageProp baseProp 9999)
