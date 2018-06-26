DECLARE @supplier VARCHAR(50)
DECLARE @ProductCode_ID VARCHAR(100)
DECLARE @BatchNumber_str VARCHAR(100)
DECLARE @Expiry_dat DATETIME
DECLARE @QtyOnHand_int INT
DECLARE @unit_code VARCHAR(50)
DECLARE @products_cursor CURSOR
DECLARE @y_cursor CURSOR
DECLARE @facility_code as Varchar(100)

SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)

-- PICK ALL PRODUCTS FROM SOH TABLE
	SET @products_cursor = CURSOR FOR
				SELECT A.ProductCode_ID, A.BatchNumber_str, A.Expiry_dat, A.QtyOnHand_int FROM dbo.TblProductBatch AS A INNER JOIN (SELECT * FROM dbo.tblProductPackSize) AS B ON A.ProductCode_ID = B.ProductCode_ID WHERE B.NSN_str <> ''
			  OPEN  @products_cursor
			  FETCH NEXT 
			  FROM  @products_cursor into @ProductCode_ID, @BatchNumber_str, @Expiry_dat, @QtyOnHand_int
			  WHILE  @@fetch_status = 0
			  BEGIN
					SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @ProductCode_ID 
					SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID WHERE (dbo.TblAudit.ProductCode_ID = @ProductCode_ID)
					IF(@Supplier = '3')
						INSERT INTO FDMS_INTERFACE.dbo.intf_store_stock
						        ( intf_facility_code ,
						          cdc_date ,
						          intf_product_code ,
						          intf_unit_code ,
						          batch_number ,
						          quantity ,
						          manufacture_date ,
						          expiry_date ,
						          add_date ,
						          sync_status
						        )
						VALUES  ( @facility_code , -- intf_facility_code - varchar(100)
						          GETDATE() , -- cdc_date - datetime
						          @ProductCode_ID , -- intf_product_code - varchar(100)
						          @unit_code , -- intf_unit_code - varchar(100)
						          @BatchNumber_str , -- batch_number - varchar(100)
						          @QtyOnHand_int , -- quantity - real
						          '' , -- manufacture_date - date
						          @Expiry_dat , -- expiry_date - date
						          GETDATE() , -- add_date - datetime
						          0  -- sync_status - int
						        )
			  FETCH  NEXT 
		      FROM  @products_cursor into @ProductCode_ID, @BatchNumber_str, @Expiry_dat, @QtyOnHand_int
		      END 
		      CLOSE @products_cursor
		      DEALLOCATE @products_cursor


