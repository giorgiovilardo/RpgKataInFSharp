module Character

open System.Linq

type Status =
    | Alive
    | Dead

type PropStatus =
    | Intact
    | Destroyed

type Prop =
    { Name: string
      Health: int
      PropStatus: PropStatus }

type CharacterStats =
    { Health: int
      Level: int
      Status: Status
      Range: int }

type Character =
    { Name: string
      Faction: string list option
      Stats: CharacterStats }

type Entity =
    | Character of Character
    | Prop of Prop

let joinFaction char faction =
    match char.Faction with
    | None -> { char with Faction = Some [ faction ] }
    | Some x ->
        { char with
              Faction = Some(faction :: x) }

let leaveFaction char faction =
    match char.Faction with
    | None -> char
    | Some factionList ->
        match factionList.Length with
        | 1 -> { char with Faction = None }
        | _ ->
            { char with
                  Faction = Some(factionList |> List.filter (fun f -> f <> faction)) }

let isAlly sourceChar destChar =
    match sourceChar.Faction, destChar.Faction with
    | Some x, Some y -> x.Intersect(y).Count() <> 0
    | _ -> false

let normalizeDamage sourceChar destChar damage =
    match destChar.Level - sourceChar.Level with
    | levelDelta when levelDelta >= 5 -> damage / 2
    | levelDelta when levelDelta <= -5 -> damage * 2
    | _ -> damage

let checkIfCharIsInRange sourceChar destChar =
    match sourceChar.Stats.Range, destChar.Stats.Range with
    | sourceRange, destRange when sourceRange < destRange -> false
    | _ -> true

let private healthInteractor (fn: int -> int -> int) (char: CharacterStats) qty =
    { char with
          Health = fn char.Health qty }

let private subtractHealth = healthInteractor (-)

let private addHealth = healthInteractor (+)

let private sanitizeStatus character =
    match character.Stats.Status with
    | Dead ->
        { character with
              Stats = { character.Stats with Health = 0 } }
    | Alive ->
        match character.Stats.Health with
        | hp when hp < 0 ->
            { character with
                  Stats =
                      { character.Stats with
                            Health = 0
                            Status = Dead } }
        | hp when hp > 1000 ->
            { character with
                  Stats = { character.Stats with Health = 1000 } }
        | _ -> character

let damageProp (destinationProp: Prop) amount =
    match destinationProp.Health - amount with
    | remainingHp when remainingHp >= 0 ->
        { destinationProp with
              Health = remainingHp }
    | _ ->
        { destinationProp with
              Health = 0
              PropStatus = Destroyed }

let damageCharacter sourceChar destinationChar amount =
    match sourceChar, destinationChar with
    | source, dest when isAlly source dest -> dest
    | source, dest when not (checkIfCharIsInRange source dest) -> dest
    | source, dest when source = dest -> dest
    | _ ->
        let damage =
            normalizeDamage sourceChar.Stats destinationChar.Stats amount

        let newStats =
            subtractHealth destinationChar.Stats damage

        sanitizeStatus
            { destinationChar with
                  Stats = newStats }

let healCharacter sourceChar destinationChar amount =
    match sourceChar = destinationChar, isAlly sourceChar destinationChar with
    | false, false -> destinationChar
    | _ ->
        let newStats = addHealth destinationChar.Stats amount

        sanitizeStatus
            { destinationChar with
                  Stats = newStats }

let damage (sourceChar: Character) (destEntity: Entity) (amount: int) : Entity =
    match destEntity with
    | Character c -> Character(damageCharacter sourceChar c amount)
    | Prop p -> Prop(damageProp p amount)

let heal (sourceChar: Character) (destEntity: Entity) (amount: int) : Entity =
    match destEntity with
    | Character c -> Character(healCharacter sourceChar c amount)
    | Prop p -> Prop p
