
local pika = service.Pokemon:Retrieve(15)

pika.Type1 = TypeId.Electric

service.Pokemon:Save()
