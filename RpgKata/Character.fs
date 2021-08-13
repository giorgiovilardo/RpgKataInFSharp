module Character

type Status =
    | Alive
    | Dead

type Character =
    { Name: string
      Health: int
      Level: int
      Status: Status }

//let private SubtractHealth char qty =
//    { char with Health = char.Health - qty }
//
//let private AddHealth char qty =
//    { char with Health = char.Health + qty }
//
//let private killCharIfHealthIsNegative char =
//    match char.Health with
//    | hp when hp < 0 -> { char with Health = 0; Status = Dead }
//    | _ -> char

let DamageChar dam char =
    match char.Health with
    | hp when hp - dam < 0 -> { char with Health = 0; Status = Dead }
    | _ -> { char with Health = char.Health - dam }

let HealChar healing char =
    match char.Status with
    | Dead -> char
    | Alive ->
        match char.Health with
        | hp when hp + healing > 1000 -> { char with Health = 1000 }
        | _ ->
            { char with
                  Health = char.Health + healing }

let normalizeDamage sourceChar destChar damage =
    match destChar.Level with
    | level when level - sourceChar.Level >= 5 -> damage / 2
    | level when level - sourceChar.Level <= -5 -> damage * 2
    | _ -> damage

let isSameCharacter sourceChar destChar = sourceChar.Name = destChar.Name

let Damage sourceCharacter damage destinationCharacter =
    match isSameCharacter sourceCharacter destinationCharacter with
    | true -> destinationCharacter
    | false ->
        let normalizedDamage =
            normalizeDamage sourceCharacter destinationCharacter damage

        DamageChar normalizedDamage destinationCharacter

let Heal sourceCharacter healing destinationCharacter =
    match isSameCharacter sourceCharacter destinationCharacter with
    | false -> destinationCharacter
    | true -> HealChar healing sourceCharacter
