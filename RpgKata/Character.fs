module Character

type Status =
    | Alive
    | Dead

type CharacterStats =
    { Health: int
      Level: int
      Status: Status
      Range: int }

type CharacterWithoutFaction = { Name: string; Stats: CharacterStats }
type CharacterWithFaction = {
    Name: string
    Faction: string
    Stats: CharacterStats
}

type Character =
    | CharacterWithFaction of CharacterWithFaction
    | CharacterWithoutFaction of CharacterWithoutFaction

let normalizeDamage sourceChar destChar damage =
    match destChar.Level with
    | level when level - sourceChar.Level >= 5 -> damage / 2
    | level when level - sourceChar.Level <= -5 -> damage * 2
    | _ -> damage

let checkIfCharIsInRange (sourceChar:Character) (destChar:Character) =
    match (sourceChar.Stats.Range, destChar.Stats.Range) with
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
    if checkIfCharIsInRange sourceChar destinationChar |> not then
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
    match sourceChar = destinationChar with
    | false -> destinationChar
    | _ ->
        let newStats = addHealth destinationChar.Stats amount
        sanitizeStatus
            { destinationChar with
                  Stats = newStats }
