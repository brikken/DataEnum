declare @int_min bigint = -2147483648
declare @int_max bigint = 2147483647

create table hbt.PITs (
	PIT int primary key
)

create table hbt.Intervals (
	loc hierarchyid primary key,
	PITStart int,
	PITEnd int,
	leaf bit
)

create index IX_leaf_PITStart on hbt.Intervals (leaf, PITStart desc)

declare @PITStart int, @PITEnd int, @PIT int, @parent hierarchyid, @oldroot hierarchyid, @count int = 0
set @PITStart = @int_min -- (@int_max - @int_min) / 3 + @int_min
set @PITEnd = @int_max -- (@int_max - @int_min) / 3 * 2 + @int_min
insert hbt.PITs values (@PITStart), (@PITEnd)
insert hbt.Intervals values (hierarchyid::GetRoot(), @PITStart, @PITEnd, 1)

--set @PIT = 0
--set @PIT = (@int_max - @int_min) / 4 + @int_min
--set @PIT = (@int_max - @int_min) / 4 * 3 + @int_min
set @PIT = rand(43784377)
while (@count < 1) begin
	set @PIT = rand() * (@int_max - @int_min) + @int_min
	if (select count(*) from hbt.PITs where PIT = @PIT) = 0 begin
		insert hbt.PITs values (@PIT)
		if @PIT < (select PITStart from hbt.Intervals where loc = hierarchyid::GetRoot()) begin
			select @PITStart = PITStart, @PITEnd = PITEnd from hbt.Intervals where loc = hierarchyid::GetRoot()
			update hbt.Intervals set loc = loc.GetReparentedValue(hierarchyid::GetRoot(), hierarchyid::GetRoot().GetDescendant(null, null))
			insert hbt.Intervals values (hierarchyid::GetRoot().GetDescendant(hierarchyid::GetRoot().GetDescendant(null, null), null), @PITEnd, @PIT, 1)
			insert hbt.Intervals values (hierarchyid::GetRoot(), @PITStart, @PIT, 0)
		end
		else if @PIT > (select PITEnd from hbt.Intervals where loc = hierarchyid::GetRoot()) begin
			select @PITStart = PITStart, @PITEnd = PITEnd from hbt.Intervals where loc = hierarchyid::GetRoot()
			update hbt.Intervals set loc = loc.GetReparentedValue(hierarchyid::GetRoot(), hierarchyid::GetRoot().GetDescendant(hierarchyid::GetRoot().GetDescendant(null, null), null))
			insert hbt.Intervals values (hierarchyid::GetRoot().GetDescendant(null, null), @PIT, @PITStart, 1)
			insert hbt.Intervals values (hierarchyid::GetRoot(), @PIT, @PITEnd, 0)
		end
		else begin
			set @parent = (select top 1 loc from hbt.Intervals where leaf = 1 and PITStart < @PIT order by PITStart desc)
			insert hbt.Intervals select i.loc.GetDescendant(null, null), i.PITStart, @PIT, 1 from hbt.Intervals i where loc = @parent
			insert hbt.Intervals select i.loc.GetDescendant(i.loc.GetDescendant(null, null), null), @PIT, i.PITEnd, 1 from hbt.Intervals i where loc = @parent
			update hbt.Intervals set leaf = 0 where loc = @parent
		end
	end
	set @count = @count + 1
end

--select loc.ToString(), PITStart, PITEnd from hbt.Intervals order by loc
select loc.ToString(), PITStart, PITEnd from hbt.Intervals where leaf = 1 order by PITStart
select count(*) from hbt.PITs

drop table hbt.PITs
drop table hbt.Intervals
