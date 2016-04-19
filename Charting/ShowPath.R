
library(ggplot2)

s <- "uruguay_4_0.13_100919.8"
df <- read.csv(paste("../Output/", s, ".csv", sep=""), header=TRUE)

plot <- ggplot(data=df)
plot <- plot + geom_segment(aes(x=X1, y=Y1, xend=X2, yend=Y2))
plot <- plot + geom_point(aes(x=X1, y=Y1))
plot <- plot + geom_point(aes(x=X2, y=Y2))
plot <- plot + theme(panel.background=element_rect("white"), panel.border=element_rect("darkgray", fill=NA))
plot

ggsave(filename=paste("../Charting/", s, ".jpeg", sep=""), plot)
