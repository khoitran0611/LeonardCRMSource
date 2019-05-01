DELETE Eli_EntityFields WHERE (FieldName = 'DeliveryStreet' OR FieldName = 'DeliveryCity' OR FieldName = 'DeliveryState' OR FieldName = 'DeliveryZip') AND ModuleId = 2

ALTER TABLE SalesCustomer DROP COLUMN DeliveryStreet, DeliveryCity, DeliveryState, DeliveryZip
