

--trig_rx_patient_visit
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_patient_visit]'))
DROP TRIGGER [dbo].[trig_rx_patient_visit]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_patient_visit]
ON [dbo].[TblAuditDispensing] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @sex as Varchar(100)
	DECLARE @age as int
	DECLARE @visit_date as datetime
	DECLARE @product_code as int
	DECLARE @unit_code as nvarchar(100)
	DECLARE @batch_number as Varchar(100)
	DECLARE @quantity as int
	DECLARE @art_start_date as datetime
	DECLARE @regimen_code as Varchar(100)
	DECLARE @patient_category as Varchar(100)
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @operation as Varchar(10)
	DECLARE @Reference_str as Varchar(100)
	DECLARE @Count as int
	DECLARE @patient_code as varchar(100)
--	DECLARE @patientcode as varchar(512)
	DECLARE @patientcode as varchar(100)
	DECLARE @HashThis varchar(255)
	DECLARE @ProductCode as varchar(100)
	DECLARE @Supplier as VARCHAR(100)

	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)
	
	SET @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')

	SET @cdc_date = getdate()
	SET @add_date= @cdc_date
	SET @sync_status = 0
	SET @art_start_date = ' '
    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    IF @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
	ELSE
        BEGIN

-- trigger treats insert and update as same, so we can make it clear here 
  
--Capturing Insert Operation
     IF @operation = 'Inserted'
        BEGIN
				SELECT @product_code = I.ProductCode_ID, @Reference_str = I.Reference_str, @visit_date= I.Date_dat, @quantity = I.Quantity_int FROM INSERTED I WHERE I.Type_str='S'
				SELECT @age= datediff(hour,p.personDOB_Dat,getdate())/8766,@sex = p.personGender_Str,@patient_category = Classification_str, @patient_code = P.personRefNoCurrent_str from tblperson p where p.personRefNoCurrent_str = substring(right(rtrim(@Reference_str),14),1,12)
				SELECT @batch_number = [RxSolution].[dbo].[TblAudit].[BatchNumber_str] FROM [RxSolution].[dbo].[TblAudit] WHERE ProductCode_ID = @product_code
				SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

				-- ADDING SUPPLIER LOGIC
				SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
									WHERE (dbo.TblAudit.ProductCode_ID = @product_code) 
				-- adding regimen logic
				SELECT @regimen_code = TblRXItem.FrmFormulation_Str FROM [RxSolution].[dbo].[TblRXItem] WHERE substring(right(TblRXItem.DspLabel04_Str,13),1,12) = @patient_code
		
				--HOT WIRE
				SELECT HashBytes('MD5', @patient_code)
				SELECT @patientcode = SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5', @patient_code)), 3, 32) 

				SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)

				IF(@patient_code is not null AND @regimen_code is not null AND @Supplier = '3')					
					INSERT FDMS_INTERFACE.dbo.intf_patient_visit
							( intf_facility_code,cdc_date,intf_patient_code,sex,age,visit_date,intf_product_code,batch_number,
							  intf_unit_code,product_quantity,art_start_date,intf_regimen_code,patient_category,add_date,sync_status)  
			 
					VALUES  ( @facility_code,@cdc_date,@patientcode,@sex,@age,@visit_date,@ProductCode, @batch_number,
							  @unit_code,@quantity,@art_start_date, @regimen_code,@patient_category,@add_date,@sync_status )

				IF(@patient_code is not null AND @regimen_code is not null AND @S_Code ='MAUL')					
					INSERT FDMS_INTERFACE.dbo.intf_patient_visit
							( intf_facility_code,cdc_date,intf_patient_code,sex,age,visit_date,intf_product_code,batch_number,
							  intf_unit_code,product_quantity,art_start_date,intf_regimen_code,patient_category,add_date,sync_status)  
			 
					VALUES  ( @facility_code,@cdc_date,@patientcode,@sex,@age,@visit_date,@ProductCode, @batch_number,
							  @unit_code,@quantity,@art_start_date, @regimen_code,@patient_category,@add_date,@sync_status )

		END

-- Capture Update Operation
ELSE
        BEGIN
				SELECT @product_code = I.ProductCode_ID, @Reference_str = I.Reference_str, @visit_date= I.Date_dat, @quantity = I.Quantity_int FROM INSERTED I WHERE I.Type_str='S'
				SELECT @age= datediff(hour,p.personDOB_Dat,getdate())/8766,@sex = p.personGender_Str,@patient_category = Classification_str, @patient_code = P.personRefNoCurrent_str from tblperson p where p.personRefNoCurrent_str = substring(right(rtrim(@Reference_str),14),1,12)
				SELECT @batch_number = [RxSolution].[dbo].[TblAudit].[BatchNumber_str] FROM [RxSolution].[dbo].[TblAudit] WHERE ProductCode_ID = @product_code
				SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

				-- ADDING SUPPLIER LOGIC
				SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
									WHERE (dbo.TblAudit.ProductCode_ID = @product_code) 
				-- adding regimen logic
				SELECT @regimen_code = TblRXItem.FrmFormulation_Str FROM [RxSolution].[dbo].[TblRXItem] WHERE substring(right(TblRXItem.DspLabel04_Str,13),1,12) = @patient_code
		
				--HOT WIRE
				SELECT HashBytes('MD5', @patient_code)
				SELECT @patientcode = SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5', @patient_code)), 3, 32) 

				SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)

				IF(@patient_code is not null AND @regimen_code is not null AND @Supplier = '3')					
					INSERT FDMS_INTERFACE.dbo.intf_patient_visit
							( intf_facility_code,cdc_date,intf_patient_code,sex,age,visit_date,intf_product_code,batch_number,
							  intf_unit_code,product_quantity,art_start_date,intf_regimen_code,patient_category,add_date,sync_status)  
			 
					VALUES  ( @facility_code,@cdc_date,@patientcode,@sex,@age,@visit_date,@ProductCode, @batch_number,
							  @unit_code,@quantity,@art_start_date, @regimen_code,@patient_category,@add_date,@sync_status )

				IF(@patient_code is not null AND @regimen_code is not null AND @S_Code ='MAUL')					
					INSERT FDMS_INTERFACE.dbo.intf_patient_visit
							( intf_facility_code,cdc_date,intf_patient_code,sex,age,visit_date,intf_product_code,batch_number,
							  intf_unit_code,product_quantity,art_start_date,intf_regimen_code,patient_category,add_date,sync_status)  
			 
					VALUES  ( @facility_code,@cdc_date,@patientcode,@sex,@age,@visit_date,@ProductCode, @batch_number,
							  @unit_code,@quantity,@art_start_date, @regimen_code,@patient_category,@add_date,@sync_status )

		END
END

GO



--trig_rx_facility_order

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_facility_order]'))
DROP TRIGGER [dbo].[trig_rx_facility_order]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_facility_order]
ON [dbo].[TblAudit] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @order_date as datetime
	DECLARE @edd_date as date
	DECLARE @product_code as int
	DECLARE @unit_code as Varchar(100)
	DECLARE @quantity as int
	DECLARE @receipt_id as int
	DECLARE @supplier_code as Varchar(100)
	DECLARE @receipt_number as Varchar(100)
	DECLARE @order_number as int
	DECLARE @expiry_date as datetime
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @OrderNo_str as varchar(100)
	DECLARE @operation as Varchar(10)
	DECLARE @Count as int
	DECLARE @ProductCode as varchar(50)
	DECLARE @Order_ref as VARCHAR(100)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = getdate()
	SET @add_date = getdate()
	SET @sync_status = 0
    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
		ELSE

    BEGIN
-- Trigger treats insert and update as same, so we can make it clear here 
-- Capturing Insert Operation
     if @operation = 'Inserted'

        BEGIN
			SELECT @OrderNo_str = INSERTED.Reference_str,@product_code=INSERTED.ProductCode_ID,@quantity= INSERTED.Quantity_int,@expiry_date=INSERTED.Expiry_dat, @Order_ref=INSERTED.VoucherNo_str from  INSERTED WHERE INSERTED.Type_str='O'
		  	SELECT @order_number = TblOrder.OrderNo_ID,@Supplier_code = TblOrder.Supplier_ID, @edd_date= TblOrder.ExpDelivery_dat, @order_date = TblOrder.Order_dat from TblOrder where TblOrder.OrderNo_str=@OrderNo_str	    					
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 
			SELECT @edd_date = dbo.TblOrder.ExpDelivery_dat FROM  dbo.TblOrder INNER JOIN dbo.TblOrderItems ON dbo.TblOrder.OrderNo_ID = dbo.TblOrderItems.OrderNo_ID INNER JOIN dbo.tblProductPackSize ON dbo.TblOrderItems.ProductCode_ID = dbo.tblProductPackSize.ProductCode_ID WHERE (dbo.tblProductPackSize.ProductCode_ID = @product_code)

			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier_code)

			IF(@OrderNo_str IS NOT NULL AND @supplier_code = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_facility_order
						( intf_facility_code, cdc_date, order_date, edd_date, intf_product_code, 
						    intf_unit_code, quantity, supplier_code, order_number, add_date, sync_status, whs_order_ref)
			 
			  VALUES (@facility_code,@cdc_date,@order_date,@edd_date,@ProductCode, 
						@unit_code,@quantity,@supplier_code,@OrderNo_str,@add_date,@sync_status, @Order_ref )

			ELSE IF (@OrderNo_str IS NOT NULL AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_facility_order
						( intf_facility_code, cdc_date, order_date, edd_date, intf_product_code,
						  intf_unit_code, quantity, supplier_code, order_number, add_date, sync_status, whs_order_ref)  
			 
			  VALUES (@facility_code,@cdc_date,@order_date,@edd_date,@ProductCode, 
						@unit_code,@quantity,(CASE 
														WHEN @supplier_code <3 THEN '3'
														WHEN @supplier_code >3 THEN '3'
														ELSE '' END),@OrderNo_str,@add_date,@sync_status, @Order_ref )

		END
		ELSE

-- Capture Update Operation
        BEGIN
		
			SELECT   @OrderNo_str = INSERTED.Reference_str,@product_code=INSERTED.ProductCode_ID,@quantity= INSERTED.Quantity_int,@expiry_date=INSERTED.Expiry_dat, @Order_ref=INSERTED.VoucherNo_str from  INSERTED WHERE INSERTED.Type_str='O'
		  	SELECT @order_number = TblOrder.OrderNo_ID,@Supplier_code = TblOrder.Supplier_ID, @edd_date= TblOrder.ExpDelivery_dat, @order_date = TblOrder.Order_dat from TblOrder where TblOrder.OrderNo_str=@OrderNo_str
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 	
			SELECT @edd_date = dbo.TblOrder.ExpDelivery_dat FROM  dbo.TblOrder INNER JOIN dbo.TblOrderItems ON dbo.TblOrder.OrderNo_ID = dbo.TblOrderItems.OrderNo_ID INNER JOIN dbo.tblProductPackSize ON dbo.TblOrderItems.ProductCode_ID = dbo.tblProductPackSize.ProductCode_ID WHERE (dbo.tblProductPackSize.ProductCode_ID = @product_code)

			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier_code)

			IF(@OrderNo_str IS NOT NULL AND @supplier_code = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_facility_order
						( intf_facility_code, cdc_date, order_date, edd_date, intf_product_code, 
						    intf_unit_code, quantity, supplier_code, order_number, add_date, sync_status, whs_order_ref)
			 
			  VALUES (@facility_code,@cdc_date,@order_date,@edd_date,@ProductCode, 
						@unit_code,@quantity,@supplier_code,@OrderNo_str,@add_date,@sync_status, @Order_ref )

			ELSE IF (@OrderNo_str IS NOT NULL AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_facility_order
						( intf_facility_code, cdc_date, order_date, edd_date, intf_product_code,
						  intf_unit_code, quantity, supplier_code, order_number, add_date, sync_status, whs_order_ref)  
			 
			  VALUES (@facility_code,@cdc_date,@order_date,@edd_date,@ProductCode, 
						@unit_code,@quantity,(CASE 
														WHEN @supplier_code <3 THEN '3'
														WHEN @supplier_code >3 THEN '3'
														ELSE '' END),@OrderNo_str,@add_date,@sync_status, @Order_ref )
														
        END
END

GO

--trig_rx_goods_received

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_goods_received]'))
DROP TRIGGER [dbo].[trig_rx_goods_received]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_goods_received]
ON [dbo].[TblAudit] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @receipt_date as datetime
	DECLARE @product_code as int
	DECLARE @unit_code as Varchar(100)
	DECLARE @batch_number as Varchar(100)
	DECLARE @quantity as int
	DECLARE @receipt_id as int
	DECLARE @supplier_code as Varchar(100)
	DECLARE @receipt_number as Varchar(100)
	DECLARE @order_number as VARCHAR(100)
	DECLARE @manufacture_date as date
	DECLARE @expiry_date as datetime
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @ReceiptNo_str as varchar(100)
	DECLARE @Reference_str as varchar(100)
	DECLARE @product_code_audit as int
	DECLARE @operation as Varchar(10)
	DECLARE @Count as int
	DECLARE @ProductCode as varchar(50)
	DECLARE @Invoice_ref as VARCHAR(100)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)	

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = getdate()
	SET @manufacture_date =''
	SET @add_date= @cdc_date
	SET @sync_status = 0
    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
		ELSE
        BEGIN

-- trigger treats insert and update as same, so we can make it clear here 
     
--Capturing Insert Operation
     if @operation = 'Inserted'

        BEGIN
			SELECT @ReceiptNo_str = INSERTED.Reference_str,@product_code=INSERTED.ProductCode_ID,@batch_number= INSERTED.BatchNumber_str,@quantity= INSERTED.Quantity_int,@expiry_date=INSERTED.Expiry_dat from  INSERTED WHERE INSERTED.Type_str='R'
		  	SELECT @order_number = TblReceipt.OrderNo_str,@Supplier_code = TblReceipt.Supplier_ID, @receipt_id = TblReceipt.ReceiptNo_ID, @ReceiptNo_str= TblReceipt.ReceiptNo_str, @receipt_date = TblReceipt.ReceiptDate_dat, @Invoice_ref=TblReceipt.InvoiceNo_str from TblReceipt where TblReceipt.ReceiptNo_str=@ReceiptNo_str
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier_code)

			IF(@ReceiptNo_str is not null AND @supplier_code = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_goods_received
						( intf_facility_code,cdc_date,receipt_date,intf_product_code,intf_unit_code,batch_number,
						  quantity,supplier_code,receipt_number,order_number,manufacture_date,expiry_date,add_date,sync_status, whs_grn_ref)  
			 
				Values (@facility_code,@cdc_date,@receipt_date,@ProductCode , @unit_code, @batch_number, @quantity , @supplier_code, 
						@ReceiptNo_str, @order_number,@manufacture_date, @expiry_date,@add_date,@sync_status,@Invoice_ref)

			ELSE IF (@ReceiptNo_str IS NOT NULL AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_goods_received
						( intf_facility_code,cdc_date,receipt_date,intf_product_code,intf_unit_code,batch_number,
						  quantity,supplier_code,receipt_number,order_number,manufacture_date,expiry_date,add_date,sync_status, whs_grn_ref)  
			 
				Values (@facility_code,@cdc_date,@receipt_date,@ProductCode , @unit_code, @batch_number, @quantity , (CASE 
														WHEN @supplier_code <3 THEN '3'
														WHEN @supplier_code >3 THEN '3'
														ELSE '' END), 
						@ReceiptNo_str, @order_number,@manufacture_date, @expiry_date,@add_date,@sync_status,@Invoice_ref)
END

-- Capture Update Operation
ELSE

        BEGIN
			SELECT @ReceiptNo_str = INSERTED.Reference_str,@product_code=INSERTED.ProductCode_ID,@batch_number= INSERTED.BatchNumber_str,@quantity= INSERTED.Quantity_int,@expiry_date=INSERTED.Expiry_dat from  INSERTED WHERE INSERTED.Type_str='R'
		  	SELECT @order_number = TblReceipt.OrderNo_str,@Supplier_code = TblReceipt.Supplier_ID, @receipt_id = TblReceipt.ReceiptNo_ID, @ReceiptNo_str= TblReceipt.ReceiptNo_str, @receipt_date = TblReceipt.ReceiptDate_dat, @Invoice_ref=TblReceipt.InvoiceNo_str from TblReceipt where TblReceipt.ReceiptNo_str=@ReceiptNo_str
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier_code)

			IF(@ReceiptNo_str is not null AND @supplier_code = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_goods_received
						( intf_facility_code,cdc_date,receipt_date,intf_product_code,intf_unit_code,batch_number,
						  quantity,supplier_code,receipt_number,order_number,manufacture_date,expiry_date,add_date,sync_status, whs_grn_ref)  
			 
				Values (@facility_code,@cdc_date,@receipt_date,@ProductCode , @unit_code, @batch_number, @quantity , @supplier_code, 
						@ReceiptNo_str, @order_number,@manufacture_date, @expiry_date,@add_date,@sync_status,@Invoice_ref)

			ELSE IF (@ReceiptNo_str IS NOT NULL AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_goods_received
						( intf_facility_code,cdc_date,receipt_date,intf_product_code,intf_unit_code,batch_number,
						  quantity,supplier_code,receipt_number,order_number,manufacture_date,expiry_date,add_date,sync_status, whs_grn_ref)  
			 
				Values (@facility_code,@cdc_date,@receipt_date,@ProductCode , @unit_code, @batch_number, @quantity , (CASE 
														WHEN @supplier_code <3 THEN '3'
														WHEN @supplier_code >3 THEN '3'
														ELSE '' END), 
						@ReceiptNo_str, @order_number,@manufacture_date, @expiry_date,@add_date,@sync_status,@Invoice_ref)
						
        END
END

GO

--trig_rx_pharmacy_stock

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_pharmacy_stock]'))
DROP TRIGGER [dbo].[trig_rx_pharmacy_stock]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_pharmacy_stock]
ON [dbo].[tblDemanderStock] FOR INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;


	DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @manufacture_date as datetime
    DECLARE @operation as Varchar(10)
    DECLARE @Count as int
	DECLARE @unit_code as nvarchar(100)
	DECLARE @product_code as VARCHAR(100)
	DECLARE @batch_number as varchar(100)
	DECLARE @quantity as int
	DECLARE @expiry_date as datetime 
	DECLARE @add_date as Datetime
	DECLARE @sync_status as int
	DECLARE @requisition_item_id as int
	DECLARE @Supplier as Varchar(100)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)	

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = GETDATE()
	SET @sync_status = 0
	SET @add_date = GETDATE()
	SET @batch_number = null
	SET @manufacture_date ='2018-01-01'

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
	ELSE
	BEGIN
     
--Capturing Insert Operation
     if @operation = 'Inserted'
        BEGIN
            SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOnHand_int FROM inserted
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @product_code = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 
			--ADDING LOGIC
			SELECT   @batch_number = dbo.TblAudit.BatchNumber_str FROM  dbo.TblProductBatch INNER JOIN dbo.TblAudit ON dbo.TblProductBatch.ProductCode_ID = dbo.TblAudit.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code)
			SELECT   @expiry_date = dbo.TblAudit.Expiry_dat FROM   dbo.TblProductBatch INNER JOIN dbo.TblAudit ON dbo.TblProductBatch.ProductCode_ID = dbo.TblAudit.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code)

			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
							WHERE (dbo.TblAudit.ProductCode_ID = @product_code)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			IF((@product_code is not NULL AND @quantity is not null) AND @Supplier = '3')
				INSERT INTO FDMS_INTERFACE.dbo.intf_pharmacy_stock
					( intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date, add_date, sync_status)

				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)

			IF((@product_code is not NULL AND @quantity is not null) AND @S_Code ='MAUL')
				INSERT INTO FDMS_INTERFACE.dbo.intf_pharmacy_stock
					( intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date, add_date, sync_status)

				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
        END
-- Capture Update Operation
ELSE
        BEGIN
			SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOnHand_int FROM inserted
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @product_code = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 
			--ADDING EXPIRY LOGIC
			SELECT   @batch_number = dbo.TblAudit.BatchNumber_str FROM   dbo.TblProductBatch INNER JOIN dbo.TblAudit ON dbo.TblProductBatch.ProductCode_ID = dbo.TblAudit.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code)
			SELECT   @expiry_date = dbo.TblAudit.Expiry_dat FROM   dbo.TblProductBatch INNER JOIN dbo.TblAudit ON dbo.TblProductBatch.ProductCode_ID = dbo.TblAudit.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code)
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
							WHERE (dbo.TblAudit.ProductCode_ID = @product_code)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			IF((@product_code is not NULL AND @quantity is not null) AND @Supplier = '3')
				INSERT INTO FDMS_INTERFACE.dbo.intf_pharmacy_stock
					( intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date, add_date, sync_status)

				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)

			IF((@product_code is not NULL AND @quantity is not null) AND @S_Code ='MAUL')
				INSERT INTO FDMS_INTERFACE.dbo.intf_pharmacy_stock
					( intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date, add_date, sync_status)

				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
        END
END


GO

--trig_rx_stock_adjustment

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_adjustment]'))
DROP TRIGGER [dbo].[trig_rx_stock_adjustment]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_stock_adjustment]
ON [dbo].[TblProductVariance] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;
-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @manufacture_date as datetime
	DECLARE @operation as Varchar(10)
	DECLARE @Count as int
	DECLARE @adjustment_type as varchar(100)
	DECLARE @adjustment_date as datetime
	DECLARE @adjustment_reason as varchar(100)
	DECLARE @product_code as int
	DECLARE @unit_code as nvarchar(50)
	DECLARE @quantity as int
	DECLARE @old_qty as int
	DECLARE @new_qty as int
	DECLARE @batch_number as varchar(50)
	DECLARE @expiry_date as datetime
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @ProductCode as varchar(100)
	DECLARE @Supplier as varchar(100)
	DECLARE @type_str as varchar(50)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)
	
	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = getdate()
	SET @manufacture_date =''
	SET @add_date= @cdc_date
	SET @sync_status = 0

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
		ELSE
        BEGIN

     if @operation = 'Inserted'

        BEGIN
			
			SELECT @adjustment_type = INSERTED.Type_str, @adjustment_date = INSERTED.Date_dat, @type_str = INSERTED.Type_str , @old_qty = INSERTED.OldQty_int, @new_qty = INSERTED.NewQty_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.ExpiryDate_dat, @product_code = INSERTED.ProductCode_ID  from INSERTED 
			SET    @quantity = -1 *(@old_qty - @new_qty)
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID   FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

			-- Adding Supplier logic
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID AND dbo.TblAudit.BatchNumber_str = dbo.TblProductBatch.BatchNumber_str
								WHERE (dbo.TblAudit.ProductCode_ID = @product_code) AND (dbo.TblAudit.BatchNumber_str = @batch_number)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			-- TRANSCODE ADJUSTMENT TYPES
			SET @Adjustment_type = (
			CASE 
				WHEN 
					(@quantity) < 0 THEN 'NEGATIVE' ELSE 'POSITIVE' 
				END)
							
			IF(@Supplier = '3' OR @adjustment_type = 'T')	
		 	INSERT FDMS_INTERFACE.dbo.intf_stock_adjustment
					  (   
					     intf_facility_code,cdc_date,adjustment_type,adjustment_reason,adjustment_date,intf_product_code,
						 batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,add_date,sync_status)  			 
				Values (		
						@facility_code,@cdc_date,@adjustment_type,@type_str,@adjustment_date,@ProductCode,
						@batch_number,@unit_code,@quantity,@manufacture_date, @expiry_date,@add_date,@sync_status )
						
			IF(@S_Code ='MAUL' OR @adjustment_type = 'T')	
		 	INSERT FDMS_INTERFACE.dbo.intf_stock_adjustment
					  (   
					     intf_facility_code,cdc_date,adjustment_type,adjustment_reason,adjustment_date,intf_product_code,
						 batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,add_date,sync_status)  			 
				Values (		
						@facility_code,@cdc_date,@adjustment_type,@adjustment_reason,@adjustment_date,@ProductCode,
						@batch_number,@unit_code,@quantity,@manufacture_date, @expiry_date,@add_date,@sync_status )			
						
		END

-- Capture Update Operation
        ELSE
        BEGIN

			SELECT @adjustment_type = INSERTED.Type_str, @adjustment_date = INSERTED.Date_dat, @type_str = INSERTED.Type_str , @old_qty = INSERTED.OldQty_int, @new_qty = INSERTED.NewQty_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.ExpiryDate_dat, @product_code = INSERTED.ProductCode_ID  from INSERTED 
			SET    @quantity = -1 *(@old_qty - @new_qty)
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID   FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 

			-- Adding Supplier logic
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID AND dbo.TblAudit.BatchNumber_str = dbo.TblProductBatch.BatchNumber_str
								WHERE (dbo.TblAudit.ProductCode_ID = @product_code) AND (dbo.TblAudit.BatchNumber_str = @batch_number)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			-- TRANSCODE ADJUSTMENT TYPES
			SET @Adjustment_type = (
			CASE 
				WHEN 
					(@quantity) < 0 THEN 'NEGATIVE' ELSE 'POSITIVE' 
				END)
							
			IF(@Supplier = '3' OR @adjustment_type = 'T')	
		 	INSERT FDMS_INTERFACE.dbo.intf_stock_adjustment
					  (   
					     intf_facility_code,cdc_date,adjustment_type,adjustment_reason,adjustment_date,intf_product_code,
						 batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,add_date,sync_status)  			 
				Values (		
						@facility_code,@cdc_date,@adjustment_type,@type_str,@adjustment_date,@ProductCode,
						@batch_number,@unit_code,@quantity,@manufacture_date, @expiry_date,@add_date,@sync_status )
						
			IF(@S_Code ='MAUL' OR @adjustment_type = 'T')	
		 	INSERT FDMS_INTERFACE.dbo.intf_stock_adjustment
					  (   
					     intf_facility_code,cdc_date,adjustment_type,adjustment_reason,adjustment_date,intf_product_code,
						 batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,add_date,sync_status)  			 
				Values (		
						@facility_code,@cdc_date,@adjustment_type,@adjustment_reason,@adjustment_date,@ProductCode,
						@batch_number,@unit_code,@quantity,@manufacture_date, @expiry_date,@add_date,@sync_status )
		
        END
END
GO


--trig_rx_stock_dispensed
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_dispensed]'))
DROP TRIGGER [dbo].[trig_rx_stock_dispensed]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_stock_dispensed]
ON [RxMaster].[dbo].[TblAuditDispensing] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @sex as Varchar(100)
	DECLARE @age as int
	DECLARE @dispense_date as datetime
	DECLARE @product_code as int
	DECLARE @unit_code as nvarchar(100)
	DECLARE @batch_number as Varchar(100)
	DECLARE @quantity as int
	DECLARE @art_start_date as datetime
	DECLARE @regimen_code as Varchar(100)
	DECLARE @patient_category as Varchar(100)
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @operation as Varchar(10)
	DECLARE @Reference_str as Varchar(100)
	DECLARE @Count as int
	DECLARE @patient_code as varchar(100)
	DECLARE @patientcode as varchar(100)
	DECLARE @HashThis varchar(255)
	DECLARE @manufacture_date as datetime
	DECLARE @expiry_date as datetime
	DECLARE @pack_size as int 
	DECLARE @product_qty as int
	DECLARE @ProductCode as varchar(100)
	DECLARE @Supplier as VARCHAR(100)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @cdc_date = getdate()
	SET @add_date= @cdc_date
	SET @sync_status = 0
	SET @manufacture_date =''

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    IF @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
		ELSE

-- trigger treats insert and update as same, so we can make it clear here 
--Capturing Insert Operation

        BEGIN
		IF @operation = 'Inserted'
        BEGIN
			SELECT @product_code = I.ProductCode_ID, @Reference_str = I.Reference_str, @dispense_date= I.Date_dat, @quantity = I.Quantity_int from  INSERTED I WHERE I.Type_str='S'
			SELECT @age= datediff(hour,p.personDOB_Dat,getdate())/8766,@sex = p.personGender_Str,@patient_category = Classification_str, @patient_code = P.personRefNoCurrent_str from tblperson p where p.personRefNoCurrent_str = substring(right(rtrim(@Reference_str),14),1,12)
			SELECT @batch_number = [RxSolution].[dbo].[TblAudit].[BatchNumber_str] from [RxSolution].[dbo].[TblAudit] where ProductCode_ID = @product_code
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl,@pack_size = dbo.tblProductPackSize.PackSize_str, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 
			SELECT @regimen_code = TblRXItem.FrmFormulation_Str from [RxSolution].[dbo].[TblRXItem] where substring(right(TblRXItem.DspLabel04_Str,13),1,12) = @patient_code		
			-- ADDING EXPIRY LOGIC
			SELECT @expiry_date = Expiry_dat FROM tblAudit WHERE ProductCode_ID = @product_code
			SET @product_qty = @quantity * @pack_size
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier = MIN(dbo.TblSupplier.Name_str) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID WHERE (dbo.TblAudit.BatchNumber_str = @batch_number)
			-- HASH PATIENT CODE
			SELECT HashBytes('MD5', @patient_code)
			SELECT @patientcode = SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5', @patient_code)), 3, 32) 

			IF((@dispense_date is not null AND @patient_code is not null) AND @product_code is not null)							
			INSERT FDMS_INTERFACE.dbo.intf_stock_dispensed
						( intf_facility_code,cdc_date,dispense_date,intf_product_code,intf_unit_code,batch_number,
						quantity,patient_code,manufacture_date,expiry_date,	add_date,sync_status)  	 
				VALUES (
						@facility_code,@cdc_date,@dispense_date,@ProductCode, @unit_code, @batch_number,@product_qty,
						@patientcode,@manufacture_date, @expiry_date,@add_date,@sync_status )
			
				-- TURN ON DISPENSING (Conn Status)
				UPDATE FDMS_INTERFACE.dbo.intf_facility_config
					SET dispense_mode = 1
					WHERE intf_facility_code = @facility_code
		END

-- Capture Update Operation
		ELSE
        BEGIN
			SELECT @product_code = I.ProductCode_ID, @Reference_str = I.Reference_str, @dispense_date= I.Date_dat, @quantity = I.Quantity_int from  INSERTED I WHERE I.Type_str='S'
			SELECT @age= datediff(hour,p.personDOB_Dat,getdate())/8766,@sex = p.personGender_Str,@patient_category = Classification_str, @patient_code = P.personRefNoCurrent_str from tblperson p where p.personRefNoCurrent_str = substring(right(rtrim(@Reference_str),14),1,12)
			SELECT @batch_number = [RxSolution].[dbo].[TblAudit].[BatchNumber_str] from [RxSolution].[dbo].[TblAudit] where ProductCode_ID = @product_code
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl,@pack_size = dbo.tblProductPackSize.PackSize_str, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 
			SELECT @regimen_code = TblRXItem.FrmFormulation_Str from [RxSolution].[dbo].[TblRXItem] where substring(right(TblRXItem.DspLabel04_Str,13),1,12) = @patient_code		
			-- ADDING EXPIRY LOGIC
			SELECT @expiry_date = Expiry_dat FROM tblAudit WHERE ProductCode_ID = @product_code
			SET @product_qty = @quantity * @pack_size
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier = MIN(dbo.TblSupplier.Name_str) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID WHERE (dbo.TblAudit.BatchNumber_str = @batch_number)
			-- HASH PATIENT CODE
			SELECT HashBytes('MD5', @patient_code)
			SELECT @patientcode = SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5', @patient_code)), 3, 32) 

			IF((@dispense_date is not null AND @patient_code is not null) AND @product_code is not null)							
			INSERT FDMS_INTERFACE.dbo.intf_stock_dispensed
						( intf_facility_code,cdc_date,dispense_date,intf_product_code,intf_unit_code,batch_number,
						quantity,patient_code,manufacture_date,expiry_date,	add_date,sync_status)  	 
				VALUES (
						@facility_code,@cdc_date,@dispense_date,@ProductCode, @unit_code, @batch_number,@product_qty,
						@patientcode,@manufacture_date, @expiry_date,@add_date,@sync_status )
			
				-- TURN ON DISPENSING (Conn Status)
				UPDATE FDMS_INTERFACE.dbo.intf_facility_config
					SET dispense_mode = 1
					WHERE intf_facility_code = @facility_code

		END
END

GO

--trig_rx_stock_disposed
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_disposed]'))
DROP TRIGGER [dbo].[trig_rx_stock_disposed]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_stock_disposed]
ON [dbo].[TblProductVariance] AFTER INSERT, UPDATE
AS
	-- SET NOCOUNT ON added to prevent extra result sets from query execution
	SET NOCOUNT ON;
	-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @manufacture_date as datetime
	DECLARE @operation as Varchar(10)
	DECLARE @Count as int
	DECLARE @dispose_date as datetime
	DECLARE @dispose_reason as varchar(100)
	DECLARE @product_code as varchar(100)
	DECLARE @unit_code as varchar(100)
	DECLARE @quantity as int
	DECLARE @old_qty as int
	DECLARE @new_qty as int
	DECLARE @type_str as varchar(50)
	DECLARE @batch_number as varchar(100)
	DECLARE @expiry_date as datetime
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @ProductCode as varchar(100)
	DECLARE @Supplier as VARCHAR(100)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = getdate()
	SET @manufacture_date =''
	SET @add_date= @cdc_date
	SET @sync_status = 0

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    IF @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
-- CAPTURE INSERT OPERATION
	 ELSE
        BEGIN
			IF @operation = 'Inserted'
        BEGIN
			SELECT @dispose_date = INSERTED.Date_dat, @type_str = INSERTED.Type_str ,@dispose_reason = INSERTED.Reason_str, @old_qty = INSERTED.OldQty_int, @new_qty = INSERTED.NewQty_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.ExpiryDate_dat, @product_code = INSERTED.ProductCode_ID  from INSERTED		
			SET    @quantity = (@new_qty - @old_qty)*(-1)
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code 
			
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier = MIN(dbo.TblSupplier.Supplier_ID) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID WHERE (dbo.TblAudit.BatchNumber_str = @batch_number)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			IF((@type_str = 'E' OR @type_str = 'B' OR @type_str = 'L') AND @Supplier = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_stock_disposed
					  (   
					    intf_facility_code,cdc_date,dispose_date,dispose_reason,intf_product_code,
						intf_unit_code,quantity,batch_number,manufacture_date,expiry_date,add_date,sync_status)   
			  VALUES  (		
						@facility_code,@cdc_date,@dispose_date,@type_str,@ProductCode,@unit_code,
						@quantity,@batch_number,@manufacture_date, @expiry_date,@add_date,@sync_status )
			
			IF((@type_str = 'E' OR @type_str = 'B' OR @type_str = 'L') AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_stock_disposed
			          (   
			            intf_facility_code,cdc_date,dispose_date,dispose_reason,intf_product_code,
			            intf_unit_code,quantity,batch_number,manufacture_date,expiry_date,add_date,sync_status)   
			  VALUES  (		
			            @facility_code,@cdc_date,@dispose_date,@type_str,@ProductCode,@unit_code,
			            @quantity,@batch_number,@manufacture_date, @expiry_date,@add_date,@sync_status )
			
		END
-- CAPTURE UPDATE OPERATION
	ELSE
        BEGIN
			SELECT @dispose_date = INSERTED.Date_dat, @dispose_reason = INSERTED.Reason_str, @old_qty = INSERTED.OldQty_int, @new_qty = INSERTED.NewQty_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.ExpiryDate_dat, @product_code = INSERTED.ProductCode_ID  from INSERTED		
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 
			SET    @quantity = (@new_qty - @old_qty)*(-1)
			
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier = MIN(dbo.TblSupplier.Supplier_ID) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID
						WHERE (dbo.TblAudit.BatchNumber_str = @batch_number)
			
			SET @S_Code = (SELECT Code_str FROM [dbo].[TblSupplier] WHERE Supplier_ID = @supplier)
			
			IF((@type_str = 'E' OR @type_str = 'B' OR @type_str = 'L') AND @Supplier = '3')
		 	  INSERT FDMS_INTERFACE.dbo.intf_stock_disposed
					  ( intf_facility_code,cdc_date,dispose_date,dispose_reason,intf_product_code,intf_unit_code,
						quantity,batch_number,manufacture_date,expiry_date,add_date,sync_status
						            )  
			  VALUES (	@facility_code,@cdc_date,@dispose_date,@type_str,@ProductCode,@unit_code,
						@quantity,@batch_number,@manufacture_date, @expiry_date,@add_date,@sync_status )	
						
			IF((@type_str = 'E' OR @type_str = 'B' OR @type_str = 'L') AND @S_Code ='MAUL')
			  INSERT FDMS_INTERFACE.dbo.intf_stock_disposed
			          (   
			            intf_facility_code,cdc_date,dispose_date,dispose_reason,intf_product_code,
			            intf_unit_code,quantity,batch_number,manufacture_date,expiry_date,add_date,sync_status)   
			  VALUES  (		
			            @facility_code,@cdc_date,@dispose_date,@type_str,@ProductCode,@unit_code,
			            @quantity,@batch_number,@manufacture_date, @expiry_date,@add_date,@sync_status )
						
        END
END
GO

--trig_rx_stock_issue
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_issue]'))
DROP TRIGGER [dbo].[trig_rx_stock_issue]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER  [dbo].[trig_rx_stock_issue]
ON [RxMaster].[dbo].[TblAudit] AFTER INSERT, UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action
    DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @issue_date as datetime
	DECLARE @product_code as int
	DECLARE @unit_code as Varchar(100)
	DECLARE @batch_number as Varchar(100)
	DECLARE @quantity as int
	DECLARE @manufacture_date as date
	DECLARE @expiry_date as datetime
	DECLARE @add_date as datetime
	DECLARE @sync_status as int
	DECLARE @Requisition_number_str as varchar(100)
	DECLARE @issue_number varchar(50)
	DECLARE @operation as Varchar(10)
	DECLARE @Count as int
	DECLARE @ProductCode as varchar(50)
	DECLARE @Supplier as VARCHAR(100)
	
	SET @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @cdc_date = getdate()
	SET @manufacture_date =''
	SET @add_date= @cdc_date
	SET @sync_status = 0

-- Setting operation to 'Inserted'
	SET @operation = 'Inserted' 
--SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
--SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
ELSE
        BEGIN

-- trigger treats insert and update as same, so we can make it clear here 
--SELECT GETDATE(),'inserted',ID,Name,phonenumber from inserted     
--Capturing Insert Operation

     IF @operation = 'Inserted'

        BEGIN
			SELECT @Requisition_number_str = inserted.Reference_str, @issue_date = inserted.Date_dat, @product_code = inserted.ProductCode_ID, 
			       @batch_number = inserted.BatchNumber_str, @quantity = inserted.Quantity_int, @expiry_date = inserted.Expiry_dat 
				   FROM inserted WHERE INSERTED.Type_str='I'
				   
			SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code
			SET    @issue_number = @Requisition_number_str

			--Picking supplier details
			SELECT       @supplier = dbo.TblReceipt.Supplier_ID FROM dbo.TblReceiptItems INNER JOIN
                         dbo.TblReceipt ON dbo.TblReceiptItems.ReceiptNo_ID = dbo.TblReceipt.ReceiptNo_ID INNER JOIN
                         dbo.TblAudit ON dbo.TblReceiptItems.ProductCode_ID = dbo.TblAudit.ProductCode_ID AND 
                         dbo.TblReceiptItems.ProductCode_ID = @product_code

		  	IF(@Requisition_number_str is not null AND @supplier = '3')

		 	  INSERT FDMS_INTERFACE.dbo.intf_stock_issue
					 (  intf_facility_code,cdc_date,issue_number,requisition_number,issue_date ,intf_product_code,
						batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,sync_status,add_date  )  
			 
			  VALUES 
					  ( @facility_code,@cdc_date,@issue_number,@Requisition_number_str,@issue_date,@ProductCode,
						@batch_number, @unit_code, @quantity, @manufacture_date, @expiry_date,@sync_status,@add_date)
END
-- Capture Update Operation
ELSE

        BEGIN	
		SELECT @Requisition_number_str = inserted.Reference_str, @issue_date = inserted.Date_dat, @product_code = inserted.ProductCode_ID, 
		       @batch_number = inserted.BatchNumber_str, @quantity = inserted.Quantity_int, @expiry_date = inserted.Expiry_dat FROM inserted WHERE INSERTED.Type_str='I'	   
		SELECT @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code
		SET    @issue_number = @Requisition_number_str
			   
			   --Picking supplier details
			SELECT       @supplier = dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN
                         dbo.TblReceiptItems ON dbo.TblReceipt.ReceiptNo_ID = dbo.TblReceiptItems.ReceiptNo_ID INNER JOIN
                         dbo.TblAudit ON dbo.TblReceiptItems.ReceiptNo_ID = dbo.TblAudit.ProductCode_ID AND dbo.TblReceiptItems.BatchNumber_str = dbo.TblAudit.BatchNumber_str AND 
                         dbo.TblReceiptItems.ProductCode_ID = @product_code

		  	IF(@Requisition_number_str is not null AND @supplier = '3')

			   INSERT FDMS_INTERFACE.dbo.intf_stock_issue
					 (  intf_facility_code,cdc_date,issue_number,requisition_number,issue_date ,intf_product_code,
						batch_number,intf_unit_code,quantity,manufacture_date,expiry_date,sync_status,add_date  )  
			 
			  VALUES 
					  ( @facility_code,@cdc_date,@issue_number,@Requisition_number_str,@issue_date,@ProductCode,
						@batch_number, @unit_code, @quantity, @manufacture_date, @expiry_date,@sync_status,@add_date)		
        END
END

GO


--trig_rx_stock_requisition_delete
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_requisition_delete]'))
DROP TRIGGER [dbo].[trig_rx_stock_requisition_delete]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER  [dbo].[trig_rx_stock_requisition_delete]
ON [RxMaster].[dbo].[TblRequisitionItems] AFTER DELETE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action

    DECLARE @operation as Varchar(10)
    DECLARE @Count as int
	DECLARE @facility_code as varchar(100)
	DECLARE @requisition_No_str as varchar(100)
	DECLARE @requisition_id as int
	DECLARE @requisition_date as date
	DECLARE @product_code as int
	DECLARE @unit_code as varchar(100)
	DECLARE @quantity as int
	DECLARE @sync_status as int
	DECLARE @add_date as Date
	DECLARE @cdc_date as Datetime
	DECLARE @requisition_item_id as int
	declare @requisition_number as varchar(100)
	DECLARE @requisition_str as varchar(100)
	DECLARE @ProductCode as varchar(100)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @cdc_date = GETDATE()
	SET @sync_status = 0
	set @add_date = GETDATE()
	
        BEGIN

            SELECT   @product_code = DELETED.ProductCode_ID, @quantity = DELETED.QtyOrdered_int, @requisition_id = DELETED.Requisition_ID,@requisition_item_id = DELETED.requisitionitems_id FROM DELETED
			SET      @requisition_No_str = (SELECT Requisition_str FROM TblRequisition WHERE Requisition_ID = @requisition_id)
			SET      @requisition_date = (SELECT Requisition_dat FROM TblRequisition WHERE Requisition_ID = @requisition_id)
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code    			
			SELECT   @requisition_item_id = (select requisitionitems_id from DELETED)

			DELETE FROM FDMS_INTERFACE.dbo.intf_stock_requisition WHERE requisition_item_id = @requisition_item_id;
END

GO

--trig_rx_stock_requisition_insert
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_requisition_insert]'))
DROP TRIGGER [dbo].[trig_rx_stock_requisition_insert]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER  [dbo].[trig_rx_stock_requisition_insert]
ON [RxMaster].[dbo].[TblRequisitionItems] FOR INSERT
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;


    DECLARE @operation as Varchar(10)
    DECLARE @Count as int
	DECLARE @facility_code as varchar(100)
	DECLARE @requisition_No_str as varchar(100)
	DECLARE @requisition_id as int
	DECLARE @requisition_date as date
	DECLARE @product_code as VARCHAR(100)
	DECLARE @unit_code as varchar(100)
	DECLARE @quantity as int
	DECLARE @sync_status as int
	DECLARE @add_date as Date
	DECLARE @cdc_date as Datetime
	DECLARE @requisition_item_id as int
	DECLARE @ProductCode as varchar(100)
	DECLARE @Supplier as Varchar(100)
	DECLARE @batch_number as VARCHAR(100)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @cdc_date = GETDATE()
	SET @sync_status = 0
	SET @add_date = GETDATE()

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
              --SELECT @Count = COUNT(*) FROM DELETED
    if @Count > 0
        BEGIN
              --SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
	ELSE
	BEGIN
     
--Capturing Insert Operation
     if @operation = 'Inserted'
        BEGIN
            SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOrdered_int, @requisition_id = INSERTED.Requisition_ID, @batch_number = INSERTED.BatchNumber_str FROM inserted
			SET      @requisition_No_str = (Select Requisition_str from TblRequisition where Requisition_ID = @requisition_id)
			SET      @requisition_date = (Select Requisition_dat from TblRequisition where Requisition_ID = @requisition_id)
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @ProductCode = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 
			SELECT   @requisition_item_id = INSERTED.requisitionitems_id from INSERTED where Requisition_ID = @requisition_id

			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
							WHERE (dbo.TblAudit.ProductCode_ID = @product_code)
			
			IF((@requisition_No_str is not null AND @requisition_date is not null) AND @Supplier = '3')
				INSERT INTO FDMS_INTERFACE.dbo.intf_stock_requisition
					( intf_facility_code,cdc_date,requisition_number, requisition_date,intf_product_code,intf_unit_code, quantity,sync_status,add_date,requisition_item_id)

				VALUES 
					( @facility_code,@cdc_date,@requisition_No_str,@requisition_date,@ProductCode,@unit_code,@quantity,@sync_status,@add_date,@requisition_item_id)
        END
		END

GO

--trig_rx_stock_requisition_update
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_stock_requisition_update]'))
DROP TRIGGER [dbo].[trig_rx_stock_requisition_update]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER  [dbo].[trig_rx_stock_requisition_update]
ON [RxMaster].[dbo].[TblRequisitionItems] AFTER UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action

    DECLARE @operation as Varchar(10)
    DECLARE @Count as int

	DECLARE @facility_code as varchar(100)
	DECLARE @requisition_No_str as varchar(100)
	DECLARE @requisition_id as int
	DECLARE @requisition_date as date
	DECLARE @product_code as int
	DECLARE @unit_code as varchar(100)
	DECLARE @quantity as int
	DECLARE @sync_status as int
	DECLARE @add_date as Date
	DECLARE @cdc_date as Datetime
	DECLARE @requisition_item_id as int
	declare @requisition_number as varchar(100)
	DECLARE @requisition_str as varchar(100)
	DECLARE @Supplier as VARCHAR(100)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @cdc_date = GETDATE()
	SET @sync_status = 0
	set @add_date = GETDATE()
	
        BEGIN			
			SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOrdered_int, @requisition_id = INSERTED.Requisition_ID,@requisition_item_id = INSERTED.requisitionitems_id from inserted
			SET      @requisition_No_str = (Select Requisition_str from TblRequisition where Requisition_ID = @requisition_id)
			-- Pick requisition_item where requisition number = variable
			SELECT   @requisition_item_id = (select requisitionitems_id from INSERTED )
			
			SET      @requisition_date = (Select Requisition_dat from TblRequisition where Requisition_ID = @requisition_id)
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl  FROM  tblProductPackSize where  tblProductPackSize.ProductCode_ID = @product_code    			
			--SELECT   @requisition_item_id = (select requisitionitems_id from INSERTED WHERE Requisition_str = @requisition_str)
			
			-- ADDING SUPPLIER LOGIC
			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID WHERE (dbo.TblAudit.ProductCode_ID = @product_code)

			IF(@Supplier = '3')
            UPDATE FDMS_INTERFACE.dbo.intf_stock_requisition
				SET 
					intf_facility_code = @facility_code,
					cdc_date = @cdc_date,
					requisition_number = @requisition_No_str, 
					requisition_date = @requisition_date,
					intf_product_code = @product_code,
					intf_unit_code = @unit_code, 
					quantity = @quantity,
					sync_status = @sync_status,
					add_date = @add_date,
					requisition_item_id = @requisition_item_id
				          
			    WHERE requisition_item_id = @requisition_item_id;
END
GO

--trig_rx_store_return
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_store_return]'))
DROP TRIGGER [dbo].[trig_rx_store_return]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_store_return]
ON [RxMaster].[dbo].[TblAudit] FOR INSERT, UPDATE, DELETE NOT FOR REPLICATION
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution

 SET NOCOUNT ON;

-- Determine if this is an INSERT,UPDATE, or DELETE Action

    DECLARE @operation as Varchar(10)
    DECLARE @Count as int
	DECLARE @facility_code as varchar(100)
	DECLARE @cdc_date date
	DECLARE @unit_code as int
	DECLARE @ProductCode as varchar(100)
	DECLARE @BatchNumber_str as varchar(100)
	DECLARE @Expiry_dat as Datetime
	DECLARE @Quantity_int as int
	DECLARE @SortDate_dat as Datetime
	DECLARE @manufacture_date as date
	DECLARE @Supplier as VARCHAR(100) 

	SET		 @facility_code = (SELECT intf_facility_config.intf_facility_code FROM [FDMS_INTERFACE].dbo.intf_facility_config)
	SET		 @cdc_date = GETDATE()
	SET		 @manufacture_date = ' '
	SELECT   @ProductCode=INSERTED.ProductCode_ID, @BatchNumber_str=INSERTED.BatchNumber_str, @Expiry_dat=INSERTED.Expiry_dat, @Quantity_int=INSERTED.Quantity_int, @SortDate_dat=INSERTED.SortDate_dat FROM INSERTED
	SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl  FROM  tblProductPackSize WHERE  tblProductPackSize.ProductCode_ID = @ProductCode

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
    SELECT @Count = COUNT(*) FROM DELETED
    IF @Count > 0
        BEGIN
              SET @operation = 'Deleted' -- Set Operation to 'Deleted'
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END

     IF @operation = 'Inserted'

        BEGIN
			SELECT @Supplier = MIN(dbo.TblSupplier.Supplier_ID) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID
						WHERE (dbo.TblAudit.BatchNumber_str = @BatchNumber_str)

			IF(@Supplier = '3')
            INSERT INTO FDMS_INTERFACE.[dbo].[intf_stock_return] 
			 (
				intf_facility_code, cdc_date, return_number, return_date, intf_product_code, batch_number,
				intf_unit_code, quantity, manufacture_date, expiry_date, sync_status, add_date)
			SELECT 
				@facility_code, @cdc_date, Reference_str, Date_dat, @ProductCode, @BatchNumber_str,
				@unit_code, @Quantity_int, NULL as manufacture_date, @Expiry_dat, 0 as sync_status, @SortDate_dat
				FROM inserted WHERE VoucherNo_str = 'Variance Order (U)' OR VoucherNo_str = 'Variance Order (R)'
				--U is for Unusable and R for Return
		END

-- Capture Update Operation
ELSE
        BEGIN   
			SELECT @Supplier = MIN(dbo.TblSupplier.Supplier_ID) FROM dbo.TblAudit INNER JOIN dbo.TblSupplier ON dbo.TblAudit.Supplier_ID = dbo.TblSupplier.Supplier_ID
						WHERE (dbo.TblAudit.BatchNumber_str = @BatchNumber_str)

			IF(@Supplier = '3')        
			INSERT INTO FDMS_INTERFACE.[dbo].[intf_stock_return] 
			 (
				intf_facility_code, cdc_date, return_number, return_date, intf_product_code, batch_number,
				intf_unit_code, quantity, manufacture_date, expiry_date, sync_status, add_date)
			SELECT 
				@facility_code, @cdc_date, Reference_str, Date_dat, @ProductCode, @BatchNumber_str,
				@unit_code, @Quantity_int, NULL as manufacture_date, @Expiry_dat, 0 as sync_status, @SortDate_dat
				FROM inserted WHERE VoucherNo_str = 'Variance Order (U)' OR VoucherNo_str = 'Variance Order (R)'
				--U is for Unusable and R for Return
END

GO

--trig_rx_store_stock
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_rx_store_stock]'))
DROP TRIGGER [dbo].[trig_rx_store_stock]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER  [dbo].[trig_rx_store_stock]
ON [dbo].[TblProductBatch] FOR INSERT,UPDATE
AS
-- SET NOCOUNT ON added to prevent extra result sets from query execution
 SET NOCOUNT ON;
	DECLARE @facility_code as Varchar(100)
	DECLARE @cdc_date as datetime
	DECLARE @manufacture_date as datetime
    DECLARE @operation as Varchar(10)
    DECLARE @Count as int
	DECLARE @unit_code as nvarchar(100)
	DECLARE @product_code as VARCHAR(100)
	DECLARE @batch_number as varchar(100)
	DECLARE @quantity as int
	DECLARE @expiry_date as datetime 
	DECLARE @add_date as Datetime
	DECLARE @sync_status as int
	DECLARE @requisition_item_id as int
	DECLARE @Supplier as Varchar(100)
	DECLARE @Transfer_Status as VARCHAR(100)
	DECLARE @Supplier_ID VARCHAR(50)
	DECLARE @S_Code VARCHAR(20)

	SET @facility_code = (SELECT intf_facility_config.intf_facility_code from [FDMS_INTERFACE].dbo.intf_facility_config)
	SET @Supplier_ID = (SELECT Supplier_ID FROM [dbo].[TblSupplier] WHERE Code_str = 'MAUL')
	
	SET @cdc_date = GETDATE()
	SET @sync_status = 0
	SET @add_date = GETDATE()
	SET @manufacture_date = ' '

    SET @operation = 'Inserted' -- Setting operation to 'Inserted'
    IF @Count > 0
        BEGIN
              SELECT @Count = COUNT(*) FROM INSERTED
              IF @Count > 0
              SET @operation = 'Updated' -- Set Operation to 'Updated'
        END
	ELSE
BEGIN     
--Capturing Insert Operation
     IF @operation = 'Inserted'
        BEGIN
            SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOnHand_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.Expiry_dat, @Supplier = INSERTED.Supplier_ID FROM inserted
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @product_code = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 

			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
							WHERE (dbo.TblAudit.ProductCode_ID = @product_code)

			--VALIDATE Transfer-in
			SELECT @Transfer_Status = dbo.TblAudit.ProductCode_ID FROM dbo.TblAudit LEFT OUTER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code) AND (dbo.TblAudit.VoucherNo_str = 'Variance Order (T)')

			IF(@Supplier = '3' OR @Transfer_Status = @product_code)
				INSERT INTO FDMS_INTERFACE.dbo.intf_store_stock
					(intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date,add_date,sync_status)
				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
					
			IF(@S_Code ='MAUL' OR @Transfer_Status = @product_code)
				INSERT INTO FDMS_INTERFACE.dbo.intf_store_stock
					(intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date,add_date,sync_status)
				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
					
        END
	 ELSE
        BEGIN
            SELECT   @product_code = INSERTED.ProductCode_ID, @quantity = INSERTED.QtyOnHand_int, @batch_number = INSERTED.BatchNumber_str, @expiry_date = INSERTED.Expiry_dat, @Supplier = INSERTED.Supplier_ID FROM inserted
			SELECT   @unit_code = dbo.tblProductPackSize.PackSizeValue_dbl, @product_code = dbo.tblProductPackSize.ProductCode_ID FROM tblProductPackSize WHERE tblProductPackSize.ProductCode_ID = @product_code 

			SELECT @Supplier =  dbo.TblReceipt.Supplier_ID FROM dbo.TblReceipt INNER JOIN dbo.TblAudit ON dbo.TblReceipt.Supplier_ID = dbo.TblAudit.Supplier_ID INNER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID
							WHERE (dbo.TblAudit.ProductCode_ID = @product_code)

			--VALIDATE Transfer-in
			SELECT @Transfer_Status = dbo.TblAudit.ProductCode_ID FROM dbo.TblAudit LEFT OUTER JOIN dbo.TblProductBatch ON dbo.TblAudit.ProductCode_ID = dbo.TblProductBatch.ProductCode_ID WHERE (dbo.TblProductBatch.ProductCode_ID = @product_code) AND (dbo.TblAudit.VoucherNo_str = 'Variance Order (T)')

			IF(@Supplier = '3' OR @Transfer_Status = @product_code)
				INSERT INTO FDMS_INTERFACE.dbo.intf_store_stock
					(intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date,add_date,sync_status)
				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
					
			IF(@S_Code ='MAUL' OR @Transfer_Status = @product_code)
				INSERT INTO FDMS_INTERFACE.dbo.intf_store_stock
					(intf_facility_code,cdc_date,intf_product_code,intf_unit_code,batch_number,quantity,manufacture_date,expiry_date,add_date,sync_status)
				VALUES 
					(@facility_code, @cdc_date, @product_code, @unit_code, @batch_number, @quantity, @manufacture_date, @expiry_date, @add_date, @sync_status)
        END
END



GO































