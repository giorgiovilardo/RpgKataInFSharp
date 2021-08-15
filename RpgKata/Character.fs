module Character

open System.Linq

type Status =
    | Alive
    | Dead

type CharacterStats =
    { Health: int
      Level: int
      Status: Status
      Range: int }

type Character =
    { Name: string
      Faction: string list option
      Stats: CharacterStats }

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
    | None, _ -> false
    | _, None -> false
    | Some x, Some y ->
        match x.Intersect(y).Count() with
        | 0 -> false
        | _ -> true

let normalizeDamage sourceChar destChar damage =
    match destChar.Level, sourceChar.Level with
    | targetLevel, sourceLevel when targetLevel - sourceLevel >= 5 -> damage / 2
    | targetLevel, sourceLevel when targetLevel - sourceLevel <= 5 -> damage / 2
    | _ -> damage

let checkIfCharIsInRange sourceChar destChar =
    match sourceChar.Stats.Range, destChar.Stats.Range with
    | sourceRange, destRange when sourceRange < destRange -> false
    | _ -> true

// Can this become "modifyHealth" and passing a lambda to parameterize
// the action? It looks unreadable long term tho.
let private subtractHealth char qty =
    { char with Health = char.Health - qty }

let private addHealth char qty =
    { char with Health = char.Health + qty }

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



let damageCharacter sourceChar destinationChar amount =
    if isAlly sourceChar destinationChar then
        destinationChar
    elif checkIfCharIsInRange sourceChar destinationChar
         |> not then
        destinationChar
    elif sourceChar = destinationChar then
        destinationChar
    else
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
