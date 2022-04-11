
local pika = service.Pokemon:Retrieve(15)

assert(pika.Name == "Pikachu")
assert(pika.Type1 == TypeId.Electric)
assert(pika.Hp == 20)
assert(pika.IsLegendary == false)

service.Pokemon:Save()
