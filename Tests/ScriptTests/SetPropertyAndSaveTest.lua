
local pika = service:Retrieve(PokemonId.Pikachu)

pika.Type1 = TypeId.Electric

service:Save(PokemonId.Pikachu, pika)
