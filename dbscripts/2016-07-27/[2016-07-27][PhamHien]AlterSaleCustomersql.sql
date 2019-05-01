DELETE Eli_EntityFields WHERE (FieldName = 'PhysicalCountry' OR FieldName = 'MailingCountry' OR FieldName = 'DeliveryCountry') AND ModuleId = 2

ALTER TABLE SalesCustomer DROP COLUMN PhysicalCountry, MailingCountry, DeliveryCountry