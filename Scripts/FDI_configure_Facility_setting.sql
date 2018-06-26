use FDMS_INTERFACE
GO


TRUNCATE table [dbo].[intf_facility_config]

insert into intf_facility_config 
( intf_facility_code,
  intf_system_code,
  intf_database_type,
  is_active,
  is_deleted,
  add_date,
  add_by
  )
VALUES('CK0012',
       'RXSOLUTION',
	   'MSSQL',
	    1,
		0,
		getdate(),
		1
		)