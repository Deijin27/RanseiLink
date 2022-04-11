
local itemService = service.Item

for id in luanet.each(service.Item:ValidIds()) do
    local item = itemService:Retrieve(id)
    item.Name = tostring(id)
    item.ShopPriceMultiplier = id
end