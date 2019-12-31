CREATE PROCEDURE [dbo].[sp_IncrementEventTriggerCount] @EventType nvarchar(max)
	
AS
BEGIN	
UPDATE EventTriggerCount SET TriggerCount = TriggerCount + 1 WHERE EventType = @EventType;
IF @@ROWCOUNT = 0
	INSERT EventTriggerCount(EventType, TriggerCount) VALUES (@EventType, 0)
 END