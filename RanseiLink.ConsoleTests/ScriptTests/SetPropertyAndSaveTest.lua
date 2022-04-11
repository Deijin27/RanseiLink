
local pika = service.Pokemon:Retrieve(15)

pika.Type1 = TypeId.Electric
pika.Hp = 34
pika.IsLegendary = true
pika.Evolutions:Add(PokemonId.Glaceon)

service.Pokemon:Save()
