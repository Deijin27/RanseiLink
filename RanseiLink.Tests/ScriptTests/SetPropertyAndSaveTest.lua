
local pika = service.Pokemon:Retrieve(PokemonId.Pikachu)

pika.Type1 = TypeId.Electric

service.Pokemon:Save(PokemonId.Pikachu, pika)
