module Character

type Status =
    | Alive
    | Dead

type Character =
    { Health: int
      Level: int
      Status: Status }

let private SubtractHealth char qty =
    { char with Health = char.Health - qty }

let private AddHealth char qty =
    { char with Health = char.Health + qty }

let private killCharIfHealthIsNegative char =
    match char.Health with
    | hp when hp < 0 -> { char with Health = 0; Status = Dead }
    | _ -> char

let DamageChar char dam =
    match char.Health with
    | hp when hp - dam < 0 -> { char with Health = 0; Status = Dead }
    | _ -> { char with Health = char.Health - dam }

let HealChar char healing =
    match char.Status with
    | Dead -> char
    | Alive ->
        match char.Health with
        | hp when hp + healing > 1000 -> { char with Health = 1000 }
        | _ ->
            { char with
                  Health = char.Health + healing }
